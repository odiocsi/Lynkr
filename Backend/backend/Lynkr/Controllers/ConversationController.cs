using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Lynkr.Data; 
using Lynkr.Hubs;
using Lynkr.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Lynkr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly LynkrDBContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagesController(LynkrDBContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto msgDto)
        {
            var senderId = GetCurrentUserId();
            var senderUser = await _context.Users.FindAsync(senderId);
            if (senderUser == null)
            {
                return BadRequest("Sender not found");
            }

            var newMessage = new Message
            {
                ConversationId = msgDto.ConversationId,
                SenderId = senderId,
                Content = msgDto.Content,
                SentAt = DateTimeOffset.UtcNow
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();


            msgDto.MessageId = newMessage.Id;
            msgDto.SenderId = senderId;
            msgDto.SentAt = newMessage.SentAt;
            msgDto.SenderName = senderUser.Name ?? "Unknown";
            msgDto.SenderProfilePictureUrl = senderUser.ProfilePictureUrl;

            await _hubContext.Clients.Group(msgDto.ConversationId.ToString())
                             .SendAsync("ReceiveMessage", msgDto);

            return Ok(msgDto);
        }

        private int GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userId!);
        }
    }
}

// --- DTOs ---
namespace Lynkr.Models
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public string SenderName { get; set; }
        public string? SenderProfilePictureUrl { get; set; }
    }
}