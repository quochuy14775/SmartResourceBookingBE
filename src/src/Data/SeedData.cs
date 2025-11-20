using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using src.Model;

namespace src.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();

            // ---------------------------
            // Seed Roles
            // ---------------------------
            string[] roles = { "ADMIN", "MANAGER", "USER" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<long>
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
            }

            // Tạo role Admin nếu chưa có
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<long>
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                });
            }

            // ---------------------------
            // Seed Admin User
            // ---------------------------
            var adminConfig = configuration.GetSection("AdminUser");
            var adminUserName = adminConfig["UserName"]!;
            var adminEmail = adminConfig["Email"]!;
            var adminPassword = adminConfig["Password"]!;

            var admin = await userManager.FindByNameAsync(adminUserName);
            if (admin == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}
