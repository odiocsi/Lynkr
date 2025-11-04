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
        // Fetches the post and eagerly loads the associated User/Author data
        var post = await _context.Posts
            .Include(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            return NotFound($"Post with ID {id} not found.");
        }

        // Return a simplified object to the client
        return Ok(new
        {
            post.Id,
            post.Content,
            post.CreatedAt,
            AuthorName = post.User.Name,
            AuthorProfilePic = post.User.ProfilePictureUrl
        });
    }

    // POST: api/post
    [HttpPost]
    // [Authorize] // Will be added later for security
    public async Task<IActionResult> CreatePost([FromBody] PostCreateDto postDto)
    {
        // **PLACEHOLDER:** Replace 1 with the actual user ID from the JWT token
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

        // Return a 201 Created status with the location of the new resource
        return CreatedAtAction(nameof(GetPost), new { id = newPost.Id }, newPost);
    }
}