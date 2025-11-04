namespace Lynkr.Models
{
    public class User
    {
        // Primary Key (Id)
        public int Id { get; set; }

        // Profile Information
        public string Email { get; set; } // Unique
        public string PasswordHash { get; set; } // Hashed password
        public string Name { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navigation Properties (Relationships)
        public ICollection<Post> Posts { get; set; }
        public ICollection<Like> Likes { get; set; }
        // Friendships, Conversations, etc., will be added as you build them out
    }
}