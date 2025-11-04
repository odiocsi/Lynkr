namespace Lynkr.Models
{
    /// <summary>
    /// Data Transfer Object for creating a new Post.
    /// This is what the client sends to the server.
    /// </summary>
    public class PostCreateDto
    {
        // The core text content of the post
        public string Content { get; set; }

        // Optional: URL to an image, if the post includes one
        public string ImageUrl { get; set; }
    }
}

