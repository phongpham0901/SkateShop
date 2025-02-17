using Microsoft.AspNetCore.Identity;
using SkateShop.Models;

namespace SkateShop.Services
{
    public class DatabaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser>? userManager,
            RoleManager<IdentityRole>? roleManager)
        {
            if (userManager == null || roleManager == null)
            {
                Console.WriteLine("userManager or roleManager is null => exit");
                return;
            }

            // kiểm tra xem có vai trò admin hay ko 
            var exists = await roleManager.RoleExistsAsync("admin");
            if (!exists)
            {
                Console.WriteLine("Admin role is not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            // kiểm tra xem có vai trò client hay ko 
            exists = await roleManager.RoleExistsAsync("client");
            if (!exists)
            {
                Console.WriteLine("Client role is not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("client"));
            }

            // kiểm tra xem có ít nhất một người dùng quản trị viên hay ko
            var adminUsers = await userManager.GetUsersInRoleAsync("admin");
            if (adminUsers.Any())
            {
                // Người dùng quản trị đã tồn tại
                Console.WriteLine("Admin user already exists => exit");
                return;
            }

            // tạo tài khoản admin
            var user = new ApplicationUser()
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@admin.com", 
                Email = "admin@admin.com",
                CreatedAt = DateTime.Now,
            };

            string initialPassword = "admin123";

            var result = await userManager.CreateAsync(user, initialPassword);
            if (result.Succeeded)
            {
                // Thiết lập vai trò người dùng
                await userManager.AddToRoleAsync(user, "admin");
                Console.WriteLine("Admin user created successfully! Please update the initial password!");
                Console.WriteLine("Email: " + user.Email);
                Console.WriteLine("Initial password: " + initialPassword);
            }
        }
    }
}
