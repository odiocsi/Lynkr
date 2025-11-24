using Lynkr.Models;
using Microsoft.AspNetCore.Identity;

namespace Lynkr.Seeder
{
    public static class InitialData
    {
        private static readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        // --- USERS ---
        public static readonly User UserAlice = CreateUser(
            id: 1,
            email: "alice@test.com",
            name: "Alice Admin",
            password: "P@ssw0rd1",
            picUrl: "https://placehold.co/100x100/4CAF50/white?text=A",
            createdDaysAgo: 10
        );

        public static readonly User UserBob = CreateUser(
            id: 2,
            email: "bob@test.com",
            name: "Bob Beta",
            password: "P@ssw0rd1",
            picUrl: "https://placehold.co/100x100/2196F3/white?text=B",
            createdDaysAgo: 5
        );

        public static readonly User UserCharlie = CreateUser(
            id: 3,
            email: "charlie@test.com",
            name: "Charlie Chat",
            password: "P@ssw0rd1",
            picUrl: "https://placehold.co/100x100/FF9800/white?text=C",
            createdDaysAgo: 2
        );

        // --- POSTS ---
        public static readonly Post PostAlice1 = new()
        {
            Id = 1,
            UserId = UserAlice.Id,
            Content = "First post from Alice!",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-5),
            ImageUrl = "https://placehold.co/600x400/4CAF50/white?text=Alice+Post"
        };

        public static readonly Post PostBob1 = new()
        {
            Id = 2,
            UserId = UserBob.Id,
            Content = "Bob checking in with a new post.",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-2),
            ImageUrl = "https://placehold.co/600x400/2196F3/white?text=Bob+Post"
        };

        // --- FRIENDSHIPS ---
        public static readonly Friendship FriendshipAliceBob = new()
        {
            Id = 1,
            User1Id = 1, // Alice
            User2Id = 2, // Bob
            Status = "ACCEPTED",
            ActionUserId = 1, // Alice sent the request
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-3),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-3)
        };

        public static readonly Friendship FriendshipBobCharlie = new()
        {
            Id = 2,
            User1Id = 2, // Bob
            User2Id = 3, // Charlie
            Status = "PENDING", // Charlie hasn't accepted yet
            ActionUserId = 2, // Bob sent the request
            CreatedAt = DateTimeOffset.UtcNow.AddHours(-1)
        };

        // --- HELPER ---
        private static User CreateUser(int id, string email, string name, string password, string picUrl, int createdDaysAgo)
        {
            var user = new User
            {
                Id = id,
                UserName = email, 
                NormalizedUserName = email.ToUpper(), 
                Email = email,
                NormalizedEmail = email.ToUpper(), 
                EmailConfirmed = true,
                Name = name,
                ProfilePictureUrl = picUrl,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-createdDaysAgo),
                SecurityStamp = Guid.NewGuid().ToString("D") 
            };

            user.PasswordHash = _hasher.HashPassword(user, password);

            return user;
        }
    }
}