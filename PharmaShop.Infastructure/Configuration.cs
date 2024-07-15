using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmaShop.Infastructure.Data;
using PharmaShop.Infastructure.Models;
using PharmaShop.Api.Models;
using PharmaShop.Infastructure.Entities;
namespace PharmaShop.Infastructure
{
    public static class Configuration
    {
        public static void AutoMigration(this WebApplication webApplication)
        {
            using (var scope = webApplication.Services.CreateScope())
            {
                var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    appContext.Database.MigrateAsync().Wait();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static async Task SeedData(this WebApplication webApplication, IConfiguration configuration)
        {
            using (var scope = webApplication.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Lấy section từ appsettings.json ép thành kiểu DefaultUser nếu rỗng thì tạo mới DefaultUser
                var defaultUser = configuration.GetSection("DefaultUsers")?.Get<DefaultUser>() ?? new DefaultUser();
                var defaultRole = configuration.GetValue<String>("DefaultRole") ?? "SuperAdmin";

                try
                {
                    // add role
                    if (!await roleManager.RoleExistsAsync(defaultRole))
                    {
                        await roleManager.CreateAsync(new IdentityRole(defaultRole));
                    }

                    var existUser = await userManager.FindByNameAsync(defaultUser.UserName);

                    if (existUser == null)
                    {
                        // add user
                        var user = new ApplicationUser
                        {
                            UserName = defaultUser.UserName,
                            IsActive = true,
                            AccessFailedCount = 0,
                            TypeId = 1
                        };

                        var identityUser = await userManager.CreateAsync(user, defaultUser.Password);

                        if (identityUser.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, defaultRole);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
