using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lynkr.Data;
using Lynkr.Models;
[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly LynkrDBContext _context;

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

        if (post == null)
        {
            return NotFound($"Post with ID {id} not found.");
        }

        return Ok(new
        {
            post.Id,
            post.Content,
            post.CreatedAt,
            AuthorName = post.User.Name,
            AuthorProfilePic = post.User.ProfilePictureUrl
        });
    }

    // GET: api/post/feed/{userId}
    [HttpGet("feed/{userId}")]
    public async Task<IActionResult> GetFriendsFeed(int userId)
    {
        var feedPosts = await _context.Posts
            .Include(p => p.User)
            .Where(post => _context.Friendships.Any(f =>
                (f.User1Id == userId && f.User2Id == post.UserId) ||
                (f.User2Id == userId && f.User1Id == post.UserId)
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
                IsLikedByCurrentUser = post.Likes.Any(l => l.UserId == userId)
            })
            .ToListAsync();

        return Ok(feedPosts);
    }

    // POST: api/post
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] PostCreateDto postDto)
    {
        int currentUserId = 1;

        var newPost = new Post
        {
            UserId = currentUserId,
            Content = postDto.Content,
            CreatedAt = DateTimeOffset.UtcNow,
            // ImageUrl = postDto.ImageUrl 
        };

        _context.Posts.Add(newPost);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPost), new { id = newPost.Id }, newPost);
    }

    // DELETE: api/post/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = await _context.Posts.FindAsync(id);

        if (post == null)
        {
            return NotFound($"Post with ID {id} not found.");
        }
        _context.Posts.Remove(post);

        await _context.SaveChangesAsync();
     
        return NoContent();  
    }
}

// --- DTOs ---

namespace Lynkr.Models
{
    public class PostCreateDto
    {
        public string Content { get; set; }
        public string ImageUrl { get; set; }
    }
}