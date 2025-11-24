using Microsoft.AspNetCore.Identity;

namespace Lynkr.Models
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; } 
        public string? ProfilePictureUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<Post> Posts { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}