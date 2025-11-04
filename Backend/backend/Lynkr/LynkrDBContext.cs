using Lynkr.Models;
using Lynkr.Seeder; 
using Microsoft.EntityFrameworkCore;

namespace Lynkr.Data
{
    public class LynkrDBContext : DbContext
    {
        // Constructor and DbSets (Model Definition)
        public LynkrDBContext(DbContextOptions<LynkrDBContext> options)
            : base(options)
        {
        }

        // DbSets representing the tables in your database
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<Message> Messages { get; set; }

        // This method is used to configure relationships, keys, constraints, and seed data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- CORE CONFIGURATION (Likes & User) ---

            // Configure the Composite Key for the 'Likes' table
            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.PostId, l.UserId });

            // Post to Like Relationship
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId);

            // User to Like Relationship
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Set Email as unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // --- FRIENDSHIP CONFIGURATION ---

            modelBuilder.Entity<Friendship>()
                .HasIndex(f => new { f.User1Id, f.User2Id })
                .IsUnique(); // Ensure no duplicate friendships

            // User1 Foreign Key setup
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User1)
                .WithMany()
                .HasForeignKey(f => f.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            // User2 Foreign Key setup
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            // ActionUser Foreign Key setup
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.ActionUser)
                .WithMany()
                .HasForeignKey(f => f.ActionUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- CONVERSATION/CHAT CONFIGURATION ---

            // Define the Composite Key for the junction table
            modelBuilder.Entity<ConversationParticipant>()
                .HasKey(cp => new { cp.ConversationId, cp.UserId });

            // Link Participant to Conversation
            modelBuilder.Entity<ConversationParticipant>()
                .HasOne(cp => cp.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.ConversationId);

            // Link Participant to User
            modelBuilder.Entity<ConversationParticipant>()
                .HasOne(cp => cp.User)
                .WithMany()
                .HasForeignKey(cp => cp.UserId);

            // --- MESSAGE CONFIGURATION ---

            // Link Message to Conversation
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);

            // Link Message to Sender
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- DATA SEEDING (HasData) ---

            // Users
            modelBuilder.Entity<User>().HasData(InitialData.UserAlice, InitialData.UserBob);

            // Posts
            modelBuilder.Entity<Post>().HasData(InitialData.PostAlice1, InitialData.PostBob1);

            // Likes

            // Friendships

            // Conversations

            // Conversation Participants

            // Messages
        }
    }
}