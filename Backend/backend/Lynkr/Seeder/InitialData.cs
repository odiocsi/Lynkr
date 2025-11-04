using Lynkr.Models;

namespace Lynkr.Seeder
{
    public static class InitialData
    {
        // --- USERS ---
        public static readonly User UserAlice = new()
        {
            Id = 1,
            Name = "Alice Admin",
            Email = "alice@test.com",
            PasswordHash = SeederHelper.HashPassword(new User(), "P@ssw0rd1"),
            ProfilePictureUrl = "https://placehold.co/100x100/4CAF50/white?text=A",
            CreatedAt = new DateTimeOffset(2025, 1, 1, 10, 0, 0, TimeSpan.Zero)
        };

        public static readonly User UserBob = new()
        {
            Id = 2,
            Name = "Bob Beta",
            Email = "bob@test.com",
            PasswordHash = SeederHelper.HashPassword(new User(), "P@ssw0rd1"),
            ProfilePictureUrl = "https://placehold.co/100x100/2196F3/white?text=B",
            CreatedAt = new DateTimeOffset(2025, 1, 5, 11, 0, 0, TimeSpan.Zero)
        };

        // --- POSTS ---
        public static readonly Post PostAlice1 = new()
        {
            Id = 1,
            UserId = UserAlice.Id,
            Content = "First post from Alice!",
            CreatedAt = new DateTimeOffset(2025, 1, 10, 12, 0, 0, TimeSpan.Zero),
            ImageUrl = "https://placehold.co/100x100/2196F3/white?text=B"
        };

        public static readonly Post PostBob1 = new()
        {
            Id = 2,
            UserId = UserBob.Id,
            Content = "Bob checking in with a new post.",
            CreatedAt = new DateTimeOffset(2025, 1, 11, 14, 30, 0, TimeSpan.Zero),
            ImageUrl = "https://placehold.co/100x100/2196F3/white?text=B"
        };
    }
}