namespace Lynkr.Models
{
    public class Friendship
    {
        public int Id { get; set; } // Primary Key

        // Foreign Key to the user who initiated the relationship or is User 1
        public int User1Id { get; set; }

        // Foreign Key to the other user (User 2)
        public int User2Id { get; set; }

        // Status: "PENDING", "ACCEPTED", "BLOCKED"
        public string Status { get; set; }

        // The ID of the user who performed the last action (e.g., sent the request or accepted it)
        public int ActionUserId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; } // When status changed

        // Navigation Properties
        public User User1 { get; set; }
        public User User2 { get; set; }
        public User ActionUser { get; set; }
    }
}