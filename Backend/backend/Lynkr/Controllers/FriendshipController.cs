using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lynkr.Data;
using Lynkr.Models;

namespace Lynkr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipController : ControllerBase
    {
        private readonly LynkrDBContext _context;

        private const string STATUS_PENDING = "PENDING";
        private const string STATUS_ACCEPTED = "ACCEPTED";

        public FriendshipController(LynkrDBContext context)
        {
            _context = context;
        }

        // GET: api/friendship/list/{userId}
        [HttpGet("list/{userId}")]
        public async Task<IActionResult> GetFriendsList(int userId)
        {
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

        // GET: api/friendship/pending/{userId}
        [HttpGet("pending/{userId}")]
        public async Task<IActionResult> GetPendingRequests(int userId)
        {
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
            if (requestDto.CurrentUserId == requestDto.TargetUserId)
            {
                return BadRequest("You cannot add yourself.");
            }

            var existing = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.User1Id == requestDto.CurrentUserId && f.User2Id == requestDto.TargetUserId) ||
                    (f.User1Id == requestDto.TargetUserId && f.User2Id == requestDto.CurrentUserId));

            if (existing != null)
            {
                if (existing.Status == STATUS_ACCEPTED) return BadRequest("You are already friends.");
                if (existing.Status == STATUS_PENDING) return BadRequest("A request is already pending.");
            }

            var friendship = new Friendship
            {
                User1Id = requestDto.CurrentUserId,
                User2Id = requestDto.TargetUserId,
                Status = STATUS_PENDING,
                ActionUserId = requestDto.CurrentUserId,
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
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.User1Id == acceptDto.CurrentUserId && f.User2Id == acceptDto.RequesterId) ||
                    (f.User1Id == acceptDto.RequesterId && f.User2Id == acceptDto.CurrentUserId));

            if (friendship == null) return NotFound("Request not found.");

            if (friendship.Status == STATUS_ACCEPTED) return BadRequest("Already friends.");

            if (friendship.ActionUserId == acceptDto.CurrentUserId)
            {
                return BadRequest("You cannot accept your own request.");
            }

            friendship.Status = STATUS_ACCEPTED;
            friendship.ActionUserId = acceptDto.CurrentUserId; 
            friendship.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Friend request accepted.");
        }

        // DELETE: api/friendship/delete/{currentUserId}/{otherUserId}
        [HttpDelete("delete/{currentUserId}/{otherUserId}")]
        public async Task<IActionResult> RemoveFriend([FromBody] FriendDeleteDto deleteDto)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f =>
                    (f.User1Id == deleteDto.CurrentUserId && f.User2Id == deleteDto.OtherUserId) ||
                    (f.User1Id == deleteDto.OtherUserId && f.User2Id == deleteDto.CurrentUserId));

            if (friendship == null)
            {
                return NotFound("Friendship not found.");
            }

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

// --- DTOs ---

namespace Lynkr.Models
{
    public class FriendRequestDto
    {
        public int CurrentUserId { get; set; }
        public int TargetUserId { get; set; }
    }

    public class FriendDeleteDto
    {
        public int CurrentUserId { get; set; }
        public int OtherUserId { get; set; }
    }

    public class FriendAcceptDto
    {
        public int CurrentUserId { get; set; }
        public int RequesterId { get; set; }
    }
}