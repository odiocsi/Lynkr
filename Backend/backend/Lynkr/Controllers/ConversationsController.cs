using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Lynkr.Data;
using Lynkr.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lynkr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly LynkrDBContext _context;

        public ConversationsController(LynkrDBContext context)
        {
            _context = context;
        }

        // GET: api/conversations/with/123
        // Returns the conversationId for a 1:1 chat (creates it if it does not exist).
        [HttpGet("with/{otherUserId:int}")]
        public async Task<IActionResult> GetOrCreateWithUser(int otherUserId)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == otherUserId)
                return BadRequest("Cannot create a conversation with yourself.");

            var otherUserExists = await _context.Users.AnyAsync(u => u.Id == otherUserId);
            if (!otherUserExists)
                return NotFound("User not found");

            var existing = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == currentUserId && c.User2Id == otherUserId) ||
                    (c.User1Id == otherUserId && c.User2Id == currentUserId));

            if (existing != null)
            {
                return Ok(new { conversationId = existing.Id });
            }

            var conversation = new Conversation
            {
                CreatedAt = DateTimeOffset.UtcNow,
                User1Id = currentUserId,
                User2Id = otherUserId
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            return Ok(new { conversationId = conversation.Id });
        }

        // GET: api/conversations/5/messages
        [HttpGet("{conversationId:int}/messages")]
        public async Task<IActionResult> GetMessages(int conversationId)
        {
            var currentUserId = GetCurrentUserId();

            // Ensure the current user is a participant of the conversation
            var allowed = await _context.Conversations
                .AsNoTracking()
                .AnyAsync(c => c.Id == conversationId && (c.User1Id == currentUserId || c.User2Id == currentUserId));

            if (!allowed) return Forbid();

            var messages = await _context.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .Include(m => m.Sender)
                .Select(m => new
                {
                    messageId = m.Id,
                    conversationId = m.ConversationId,
                    senderId = m.SenderId,
                    content = m.Content,
                    sentAt = m.SentAt,
                    senderName = m.Sender.Name,
                    senderProfilePictureUrl = m.Sender.ProfilePictureUrl
                })
                .ToListAsync();

            return Ok(messages);
        }

        private int GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userId!);
        }
    }
}
