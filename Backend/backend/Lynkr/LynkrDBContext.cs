using Lynkr.Models;
using Lynkr.Seeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore;

namespace Lynkr.Data
{
    public class LynkrDBContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public LynkrDBContext(DbContextOptions<LynkrDBContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CORE  ---

            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.PostId, l.UserId });

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- FRIENDSHIP  ---

            modelBuilder.Entity<Friendship>()
                .HasIndex(f => new { f.User1Id, f.User2Id })
                .IsUnique();

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User1)
                .WithMany()
                .HasForeignKey(f => f.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.User2Id)
                .OnDelete(DeleteBehavior.Restrict);


            // --- CHAT ---

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User1)
                .WithMany()
                .HasForeignKey("User1Id")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey("User2Id")
                .OnDelete(DeleteBehavior.Restrict);

            // --- SEEDING ---

            modelBuilder.Entity<User>().HasData(
                InitialData.UserAlice,
                InitialData.UserBob,
                InitialData.UserCharlie
            );

            modelBuilder.Entity<Post>().HasData(
                InitialData.PostAlice1,
                InitialData.PostBob1
            );

            modelBuilder.Entity<Friendship>().HasData(
                InitialData.FriendshipAliceBob,
                InitialData.FriendshipBobCharlie
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
    }
}