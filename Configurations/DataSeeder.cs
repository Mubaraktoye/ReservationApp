using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Configurations
{
    public class DataSeeder
    {
        public static async Task SeedAsync(BookingDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Rooms
            if (!context.Rooms.Any())
            {
                context.Rooms.AddRange(
                    new Room { Name = "Conference Room A", Description = "Main conference room", Capacity = 20 },
                    new Room { Name = "Meeting Room B", Description = "Small meeting room", Capacity = 8 },
                    new Room { Name = "Training Room C", Description = "Training and workshop room", Capacity = 30 }
                );
                await context.SaveChangesAsync();
            }

            // Seed Roles and Admin User
            if (!context.Users.Any(u => u.Role == "Admin"))
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@bookingsystem.com",
                    PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Admin@123"),
                    Role = "Admin"
                };
                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
