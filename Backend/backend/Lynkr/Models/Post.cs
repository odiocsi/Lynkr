namespace Lynkr.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign Key to the User
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // Navigation Properties
        public User User { get; set; } // The user who made the post
        public ICollection<Like> Likes { get; set; }
    }

    public class Like
    {
        public int PostId { get; set; } // Part of Composite Key, Foreign Key
        public int UserId { get; set; } // Part of Composite Key, Foreign Key
        public DateTimeOffset CreatedAt { get; set; }

        // Navigation Properties
        public Post Post { get; set; } // The post that was liked
        public User User { get; set; } // The user who made the like
    }
}