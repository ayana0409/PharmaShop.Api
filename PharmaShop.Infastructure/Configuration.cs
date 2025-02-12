using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PharmaShop.Application.Data;
using PharmaShop.Domain.Entities;
using CloudinaryDotNet;
using PharmaShop.Application.Models;
using PharmaShop.Domain.Abtract;
using PharmaShop.Application.Repositorys;
using PharmaShop.Infastructure.Repositorys;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace PharmaShop.Application
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
                    throw new Exception(ex.Message);
                }
            }
        }

        public static void RegisterDb(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PharmaShopDatabase")
                    ?? throw new InvalidOperationException("Connection not found.");

            services.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddSignInManager<SignInManager<ApplicationUser>>()
                    .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(option =>
            {
                option.Lockout.AllowedForNewUsers = true;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                option.Lockout.MaxFailedAccessAttempts = 3;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.UseSecurityTokenValidators = true;
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                };

                options.MetadataAddress = "https://accounts.google.com/.well-known/openid-configuration";

            })
            .AddGoogle(googleOptions =>
            {
                var clientID = configuration["GoogleAuthentication:ClientID"] ?? throw new ApplicationException("Invalid Google ClientID");
                var clientSecret = configuration["GoogleAuthentication:ClientSecret"] ?? throw new ApplicationException("Invalid Google ClientSecret");

                googleOptions.ClientId = clientID;
                googleOptions.ClientSecret = clientSecret;
            });

            services.AddSingleton(s =>
            {
                var cloudName = configuration["Cloudinary:CloudName"];
                var apiKey = configuration["Cloudinary:ApiKey"];
                var apiSecret = configuration["Cloudinary:ApiSecret"];

                return new Cloudinary(new Account(cloudName, apiKey, apiSecret));
            });

            services.AddAuthorization();
        }
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProductDetailRepository, ProductDetailRepository>();
            services.AddTransient<IImageRepository, ImageRepository>();
            services.AddTransient<IImportRepository, ImportRepository>();
            services.AddTransient<IImportDetailRepository, ImportDetailRepository>();
            services.AddTransient<ICartItemRepository, CartItemRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
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
