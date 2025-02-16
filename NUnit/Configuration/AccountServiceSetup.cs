using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PharmaShop.Application.Data;
using PharmaShop.Application.Repositorys;
using PharmaShop.Application.Services;
using PharmaShop.Domain.Abtract;
using PharmaShop.Domain.Entities;
namespace NUnitTest.Configuration
{
    public class AccountServiceSetup
    {
        private ServiceProvider _serviceProvider;
        public AccountService AccountService { get; private set; }
        public ApplicationDbContext Context { get; private set; }

        public async Task InitializeAsync()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Đăng ký DI
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Đăng ký đối tượng user và role
            services.AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser, IdentityRole, ApplicationDbContext>>();
            services.AddScoped<IRoleStore<IdentityRole>, RoleStore<IdentityRole, ApplicationDbContext>>();

            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<RoleManager<IdentityRole>>();
            services.AddScoped<SignInManager<ApplicationUser>>();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));
            services.AddScoped<AccountService>();

            _serviceProvider = services.BuildServiceProvider();

            // Khởi tạo giá trị ban đầu cho TestDB
            using (var scope = _serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var roles = new[] { "Admin", "User" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var user = new ApplicationUser
                {
                    Id = "TestAccount1",
                    UserName = "TestUser",
                    Email = "testuser@example.com"
                };

                var result = await userManager.CreateAsync(user, "Test@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, roles);
                }

                await dbContext.SaveChangesAsync();
            }

            this.Context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            this.AccountService = _serviceProvider.GetRequiredService<AccountService>();
        }

        public void Dispose()
        {
            _serviceProvider?.Dispose();
        }

        public void Cleanup()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                foreach (var entry in dbContext.ChangeTracker.Entries())
                {
                    entry.State = EntityState.Detached;
                }

                dbContext.Database.EnsureDeleted();
            }
        }
    }
}
