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
    public class PostController : ControllerBase
    {
        private readonly LynkrDBContext _context;

        private const string STATUS_PENDING = "PENDING";
        private const string STATUS_ACCEPTED = "ACCEPTED";

        public PostController(LynkrDBContext context)
        {
            _context = context;
        }

        // GET: api/post/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound($"Post with ID {id} not found.");


            return Ok(new
            {
                post.Id,
                post.Content,
                post.CreatedAt,
                AuthorName = post.User.Name,
                AuthorProfilePic = post.User.ProfilePictureUrl,
                LikeCount = await _context.Likes.CountAsync(l => l.PostId == id)
            });
        }

        // GET: api/post/feed
        [HttpGet("feed")]
        public async Task<IActionResult> GetFriendsFeed()
        {
            var currentUserId = GetCurrentUserId();

            var feedPosts = await _context.Posts
                .Include(p => p.User)
                .Where(post => _context.Friendships.Any(f =>
                            (
                                (f.User1Id == currentUserId && f.User2Id == post.UserId) ||
                                (f.User2Id == currentUserId && f.User1Id == post.UserId)
                            )
                            && f.Status == STATUS_ACCEPTED 
                        ))
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .Select(post => new
                {
                    post.Id,
                    post.Content,
                    post.CreatedAt,
                    AuthorId = post.UserId,
                    AuthorName = post.User.Name,
                    AuthorProfilePic = post.User.ProfilePictureUrl,
                    LikeCount = post.Likes.Count(),
                    IsLikedByCurrentUser = post.Likes.Any(l => l.UserId == currentUserId)
                })
                .ToListAsync();

            return Ok(feedPosts);
        }

        // POST: api/post/{id}/like
        // This toggles the like: If liked, it unlikes. If not liked, it likes.
        [HttpPost("{id}/like")]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var currentUserId = GetCurrentUserId();

            var postExists = await _context.Posts.AnyAsync(p => p.Id == id);
            if (!postExists) return NotFound("Post not found.");

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == id && l.UserId == currentUserId);

            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Post unliked", IsLiked = false });
            }
            else
            {
                var newLike = new Like
                {
                    PostId = id,
                    UserId = currentUserId,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                _context.Likes.Add(newLike);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Post liked", IsLiked = true });
            }
        }

        // POST: api/post
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostCreateDto postDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentUserId = GetCurrentUserId();

            var newPost = new Post
            {
                UserId = currentUserId,
                Content = postDto.Content,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = newPost.Id }, newPost);
        }

        // DELETE: api/post/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var currentUserId = GetCurrentUserId();

            var post = await _context.Posts.FindAsync(id);

            if (post == null) return NotFound("Post not found.");

            if (post.UserId != currentUserId)
            {
                return Unauthorized("You are not authorized to delete this post.");
            }

            _context.Posts.Remove(post);
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
    public class PostCreateDto
    {
        public string Content { get; set; }
    }
}