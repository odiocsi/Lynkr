namespace Lynkr.Models
{
    public class Friendship
    {
        public int Id { get; set; } 

        public int User1Id { get; set; }

        public int User2Id { get; set; }

        public string Status { get; set; }

        public int ActionUserId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public User User1 { get; set; }
        public User User2 { get; set; }
    }
}