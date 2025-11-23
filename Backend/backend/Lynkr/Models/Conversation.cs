namespace Lynkr.Models
{
    public class Conversation
    {
        public int Id { get; set; } // Primary Key

        public DateTimeOffset CreatedAt { get; set; }

        // Navigation Properties
        public User User1 { get; set; }
        public User User2 { get; set; }
        public ICollection<Message> Messages { get; set; }
    }

    public class Message
    {
        public int Id { get; set; } // Primary Key

        public int ConversationId { get; set; } // Foreign Key to the chat thread
        public int SenderId { get; set; } // Foreign Key to the user who sent it

        public string Content { get; set; }
        public DateTimeOffset SentAt { get; set; }

        // Navigation Properties
        public Conversation Conversation { get; set; }
        public User Sender { get; set; }
    }
}