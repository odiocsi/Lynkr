namespace Lynkr.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public string Content { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public User User { get; set; } 
        public ICollection<Like> Likes { get; set; }
    }

    public class Like
    {
        public int PostId { get; set; } 
        public int UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public Post Post { get; set; } 
        public User User { get; set; } 
    }
}