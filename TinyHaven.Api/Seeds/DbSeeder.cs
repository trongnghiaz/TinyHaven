using Microsoft.EntityFrameworkCore;
using TinyHaven.Api.Data;
using TinyHaven.Api.Models;

namespace TinyHaven.Api.Seeds
{
    public class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db, IConfiguration configuration)
        {
            if (await db.Users.AnyAsync())
                return;

            var users = configuration.GetSection("SeedUsers").Get<List<SeedUser>>() ?? new();

            foreach (var item in users)
            {
                db.Users.Add(new AppUser
                {
                    Username = item.Username,
                    DisplayName = item.DisplayName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(item.Password)
                });
            }

            await db.SaveChangesAsync();
        }

        public class SeedUser
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
        }

    }
}
