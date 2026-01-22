using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Lynkr.Data;
using Lynkr.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Lynkr.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly LynkrDBContext _context;

        private const string STATUS_ACCEPTED = "ACCEPTED";

        public ChatHub(LynkrDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Sends a direct (1:1) message to a specific user.
        /// No groups are used. The message is persisted in the database from this hub method.
        /// </summary>
        public async Task<MessageDto> SendDirectMessage(int recipientUserId, string content)
        {
            var senderId = GetCurrentUserId();

            if (senderId == recipientUserId)
                throw new HubException("You cannot message yourself.");

            content = (content ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(content))
                throw new HubException("Message content cannot be empty.");

            // Ensure recipient exists
            var recipientExists = await _context.Users.AsNoTracking().AnyAsync(u => u.Id == recipientUserId);
            if (!recipientExists)
                throw new HubException("Recipient not found.");

            // Ensure users are friends (ACCEPTED)
            var areFriends = await _context.Friendships.AsNoTracking().AnyAsync(f =>
                ((f.User1Id == senderId && f.User2Id == recipientUserId) ||
                 (f.User1Id == recipientUserId && f.User2Id == senderId)) &&
                f.Status == STATUS_ACCEPTED);

            if (!areFriends)
                throw new HubException("You can only message accepted friends.");

            // Find or create the DM conversation
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == senderId && c.User2Id == recipientUserId) ||
                    (c.User1Id == recipientUserId && c.User2Id == senderId));

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    CreatedAt = DateTimeOffset.UtcNow,
                    User1Id = senderId,
                    User2Id = recipientUserId
                };

                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }

            var sender = await _context.Users.FindAsync(senderId);
            if (sender == null)
                throw new HubException("Sender not found.");

            // Persist message
            var message = new Message
            {
                ConversationId = conversation.Id,
                SenderId = senderId,
                Content = content,
                SentAt = DateTimeOffset.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var dto = new MessageDto
            {
                MessageId = message.Id,
                ConversationId = conversation.Id,
                SenderId = senderId,
                Content = message.Content,
                SentAt = message.SentAt,
                SenderName = sender.Name ?? "Unknown",
                SenderProfilePictureUrl = sender.ProfilePictureUrl
            };

            await Clients.User(recipientUserId.ToString()).SendAsync("ReceiveMessage", dto);
            await Clients.User(senderId.ToString()).SendAsync("ReceiveMessage", dto);

            return dto;
        }

        private int GetCurrentUserId()
        {
            var idClaim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim)) throw new HubException("Unauthorized");
            return int.Parse(idClaim);
        }
    }
}