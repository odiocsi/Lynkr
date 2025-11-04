using Lynkr.Models;
using Microsoft.AspNetCore.Identity;

namespace Lynkr.Seeder
{
    public static class SeederHelper
    {
        private static readonly PasswordHasher<User> _passwordHasher = new();

        public static string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }
    }
}