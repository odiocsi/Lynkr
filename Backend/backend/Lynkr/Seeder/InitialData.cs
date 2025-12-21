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

        // --- CONVERSATIONS ---
        public static readonly Conversation ConversationAliceBob = new()
        {
            Id = 1,
            User1Id = 1, // Alice
            User2Id = 2, // Bob
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-4)
        };

        public static readonly Conversation ConversationAliceCharlie = new()
        {
            Id = 2,
            User1Id = 1, // Alice
            User2Id = 3, // Charlie
            CreatedAt = DateTimeOffset.UtcNow.AddHours(-2)
        };

        // --- MESSAGES ---
        public static readonly Message MsgAliceBob1 = new()
        {
            Id = 1,
            ConversationId = 1,
            SenderId = 1, // Alice
            Content = "Szia Bob! Láttad a tegnapi meccset?",
            SentAt = DateTimeOffset.UtcNow.AddDays(-4).AddHours(10)
        };

        public static readonly Message MsgAliceBob2 = new()
        {
            Id = 2,
            ConversationId = 1,
            SenderId = 2, // Bob
            Content = "Szia! Persze, elképesztő volt a vége.",
            SentAt = DateTimeOffset.UtcNow.AddDays(-4).AddHours(10).AddMinutes(5)
        };

        public static readonly Message MsgAliceBob3 = new()
        {
            Id = 3,
            ConversationId = 1,
            SenderId = 1, // Alice
            Content = "Hihetetlen, hogy megfordították. Mikor érsz rá egy kávéra?",
            SentAt = DateTimeOffset.UtcNow.AddDays(-3).AddHours(9) // Másnap
        };

        public static readonly Message MsgAliceBob4 = new()
        {
            Id = 4,
            ConversationId = 1,
            SenderId = 2, // Bob
            Content = "Holnap délután jó lehet. 4 körül?",
            SentAt = DateTimeOffset.UtcNow.AddDays(-3).AddHours(9).AddMinutes(30)
        };

        public static readonly Message MsgAliceCharlie1 = new()
        {
            Id = 5,
            ConversationId = 2,
            SenderId = 3, // Charlie
            Content = "Helló Alice! Láttam a posztodat, nagyon jó lett a kép.",
            SentAt = DateTimeOffset.UtcNow.AddHours(-2)
        };

        public static readonly Message MsgAliceCharlie2 = new()
        {
            Id = 6,
            ConversationId = 2,
            SenderId = 1, // Alice
            Content = "Köszi Charlie! :)",
            SentAt = DateTimeOffset.UtcNow.AddHours(-1).AddMinutes(55)
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