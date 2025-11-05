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


            // --- CONVERSATION/CHAT CONFIGURATION ---

            // Link Message to Conversation (One Conversation has Many Messages)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages) // <-- Ehhez a Conversation modellben vissza kell adnod az ICollection<Message> Messages-t
                .HasForeignKey(m => m.ConversationId);

            // Link Message to Sender (One User is Many Senders)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);


            // --- ÚJ Conversation Konfiguráció (1:1 Chat) ---

            // Konfiguráció User1-hez
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User1)
                .WithMany()
                .HasForeignKey("User1Id") // Feltételezve, hogy hozzáadtad a User1Id FK-t a modellhez
                .OnDelete(DeleteBehavior.Restrict);

            // Konfiguráció User2-höz
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey("User2Id") // Feltételezve, hogy hozzáadtad a User2Id FK-t a modellhez
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
    }
}