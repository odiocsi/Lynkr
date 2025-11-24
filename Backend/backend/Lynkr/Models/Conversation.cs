namespace Lynkr.Models
{
    public class Conversation
    {
        public int Id { get; set; } 

        public DateTimeOffset CreatedAt { get; set; }

        public User User1 { get; set; }
        public User User2 { get; set; }
        public ICollection<Message> Messages { get; set; }
    }

    public class Message
    {
        public int Id { get; set; } 

        public int ConversationId { get; set; } 
        public int SenderId { get; set; } 

        public string Content { get; set; }
        public DateTimeOffset SentAt { get; set; }

        public Conversation Conversation { get; set; }
        public User Sender { get; set; }
    }
}