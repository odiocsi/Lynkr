using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims; 
using Lynkr.Data;
using Lynkr.Models;

namespace Lynkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class FriendshipController : ControllerBase
    {
        private readonly LynkrDBContext _context;

        private const string STATUS_PENDING = "PENDING";
        private const string STATUS_ACCEPTED = "ACCEPTED";

        public FriendshipController(LynkrDBContext context)
        {
            _context = context;
        }

        // GET: api/friendship/list
        [HttpGet("list")]
        public async Task<IActionResult> GetFriendsList()
        {
            var userId = GetCurrentUserId(); 

            var friends = await _context.Friendships
                .AsNoTracking()
                .Where(f => (f.User1Id == userId || f.User2Id == userId) && f.Status == STATUS_ACCEPTED)
                .Include(f => f.User1)
                .Include(f => f.User2)
                .Select(f => new
                {
                    FriendId = f.User1Id == userId ? f.User2Id : f.User1Id,
                    FriendName = f.User1Id == userId ? f.User2.Name : f.User1.Name,
                    FriendProfilePic = f.User1Id == userId ? f.User2.ProfilePictureUrl : f.User1.ProfilePictureUrl,
                    FriendsSince = f.UpdatedAt ?? f.CreatedAt
                })
                .ToListAsync();

            return Ok(friends);
        }

        // GET: api/friendship/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var userId = GetCurrentUserId(); 

            var requests = await _context.Friendships
                .AsNoTracking()
                .Where(f =>
                    (f.User1Id == userId || f.User2Id == userId) &&
                    f.Status == STATUS_PENDING &&
                    f.ActionUserId != userId 
                )
                .Include(f => f.User1)
                .Include(f => f.User2)
                .Select(f => new
                {
                    RequesterId = f.ActionUserId,
  
                    RequesterName = f.User1Id == userId ? f.User2.Name : f.User1.Name,
                    RequesterPic = f.User1Id == userId ? f.User2.ProfilePictureUrl : f.User1.ProfilePictureUrl,
                    SentAt = f.CreatedAt
                })
                .ToListAsync();

            return Ok(requests);
        }

        // POST: api/friendship/request
        [HttpPost("request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDto requestDto)
        {
            var currentUserId = GetCurrentUserId(); 

            if (currentUserId == requestDto.TargetUserId)
            {
                return BadRequest("You cannot add yourself.");
            }

            var existing = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.User1Id == currentUserId && f.User2Id == requestDto.TargetUserId) ||
                    (f.User1Id == requestDto.TargetUserId && f.User2Id == currentUserId));

            if (existing != null)
            {
                if (existing.Status == STATUS_ACCEPTED) return BadRequest("You are already friends.");
                if (existing.Status == STATUS_PENDING) return BadRequest("A request is already pending.");
            }

            var friendship = new Friendship
            {
                User1Id = currentUserId,
                User2Id = requestDto.TargetUserId,
                Status = STATUS_PENDING,
                ActionUserId = currentUserId, 
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return Ok("Friend request sent.");
        }

        // PUT: api/friendship/accept
        [HttpPut("accept")]
        public async Task<IActionResult> AcceptRequest([FromBody] FriendAcceptDto acceptDto)
        {
            var currentUserId = GetCurrentUserId(); 

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.User1Id == currentUserId && f.User2Id == acceptDto.RequesterId) ||
                    (f.User1Id == acceptDto.RequesterId && f.User2Id == currentUserId));

            if (friendship == null) return NotFound("Request not found.");

            if (friendship.ActionUserId == currentUserId)
            {
                return BadRequest("You cannot accept your own request.");
            }

            if (friendship.Status == STATUS_ACCEPTED) return BadRequest("Already friends.");

            friendship.Status = STATUS_ACCEPTED;
            friendship.ActionUserId = currentUserId;
            friendship.UpdatedAt = DateTimeOffset.UtcNow;

            bool conversationExists = await _context.Conversations.AnyAsync(c =>
                (c.User1Id == currentUserId && c.User2Id == acceptDto.RequesterId) ||
                (c.User1Id == acceptDto.RequesterId && c.User2Id == currentUserId));

            if (!conversationExists)
            {
                var newConversation = new Conversation
                {
                    User1Id = currentUserId,
                    User2Id = acceptDto.RequesterId,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                _context.Conversations.Add(newConversation);
            }

            await _context.SaveChangesAsync();

            return Ok("Friend request accepted.");
        }

        // DELETE: api/friendship/delete
        [HttpDelete("delete")]
        public async Task<IActionResult> RemoveFriend([FromBody] FriendDeleteDto deleteDto)
        {
            var currentUserId = GetCurrentUserId(); 

            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.User1Id == currentUserId && f.User2Id == deleteDto.OtherUserId) ||
                    (f.User1Id == deleteDto.OtherUserId && f.User2Id == currentUserId));

            if (friendship == null)
            {
                return NotFound("Friendship not found.");
            }

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // --- HELPER ---
        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim)) throw new UnauthorizedAccessException("User ID not found in token.");
            return int.Parse(idClaim);
        }
    }
}

// --- DTOs ---
namespace Lynkr.Models
{
    public class FriendRequestDto
    {
        public int TargetUserId { get; set; }
    }

    public class FriendDeleteDto
    {
        public int OtherUserId { get; set; }
    }

    public class FriendAcceptDto
    {
        public int RequesterId { get; set; }
    }
}