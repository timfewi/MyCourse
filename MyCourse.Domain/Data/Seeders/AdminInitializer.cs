using Microsoft.AspNetCore.Identity;
using MyCourse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Data.Seeders
{
    public class AdminInitializer
    {
        public static async Task InitializeAdminAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            string[] roles = { "ADMIN" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }

            var adminUser = await userManager.FindByNameAsync("admin@example.com");
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "ADMIN");
                }
                else
                {
                    throw new Exception("Failed to create admin user.");
                }
            }
            else
            {
                // Sicherstellen, dass der Benutzer die Admin-Rolle hat
                if (!await userManager.IsInRoleAsync(adminUser, "ADMIN"))
                {
                    await userManager.AddToRoleAsync(adminUser, "ADMIN");
                }
            }
        }
    }
}
