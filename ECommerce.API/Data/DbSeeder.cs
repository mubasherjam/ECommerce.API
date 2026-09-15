using ECommerce.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Create Admin if it doesn't exist
            if (!await context.Users.AnyAsync(u => u.Email == "admin@ecommerce.com"))
            {
                var admin = new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@ecommerce.com",
                    Role = "Admin"
                };

                var passwordHasher = new PasswordHasher<User>();

                admin.PasswordHash = passwordHasher.HashPassword(
                    admin,
                    "Admin123!"
                );

                context.Users.Add(admin);
            }

            // Create Customer if it doesn't exist
            if (!await context.Users.AnyAsync(u => u.Email == "customer@ecommerce.com"))
            {
                var customer = new User
                {
                    FirstName = "Test",
                    LastName = "Customer",
                    Email = "customer@ecommerce.com",
                    Role = "Customer"
                };

                var passwordHasher = new PasswordHasher<User>();

                customer.PasswordHash = passwordHasher.HashPassword(
                    customer,
                    "Customer123!"
                );

                context.Users.Add(customer);
            }

            await context.SaveChangesAsync();
        }
    }
}