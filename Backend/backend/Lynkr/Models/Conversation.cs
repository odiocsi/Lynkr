namespace Lynkr.Models
{
    public class Conversation
    {
        public int Id { get; set; } // Primary Key

        // Used primarily for Group Chats (e.g., "The Dev Team")
        public string Name { get; set; }

        // Boolean flag to quickly distinguish 1:1 vs. Group chats
        public bool IsGroupChat { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        // Navigation Properties
        public ICollection<ConversationParticipant> Participants { get; set; }
        public ICollection<Message> Messages { get; set; }
    }

    public class ConversationParticipant
    {
        // Composite Primary Key (defined in DbContext)
        public int ConversationId { get; set; } // Foreign Key to Conversation
        public int UserId { get; set; } // Foreign Key to User

        public DateTimeOffset JoinedAt { get; set; }

        // Navigation Properties
        public Conversation Conversation { get; set; }
        public User User { get; set; }
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