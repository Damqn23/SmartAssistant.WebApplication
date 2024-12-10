using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Create Admin Role
            var adminRoleExists = await roleManager.RoleExistsAsync("Admin");
            if (!adminRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Assign the Admin Role to a User
            var user = await userManager.FindByEmailAsync("admin@example.com");
            if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
