using Microsoft.EntityFrameworkCore;
using PharmaShop.Infastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PharmaShop.Infastructure.Models;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Services;
using Microsoft.OpenApi.Models;
using PharmaShop.Application.Repositorys;
using CloudinaryDotNet;
using PharmaShop.Api.Abtract;
using PharmaShop.Api.Services;
namespace PharmaShop.Application.Setting
{
    public static class Configuration
    {
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
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"] ?? throw new ApplicationException(message: "No JWT:Secret")))
                };
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true; // Bảo vệ chống tấn công XSS
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Chỉ gửi cookie qua HTTPS
                options.Cookie.SameSite = SameSiteMode.Strict; // Chỉ gửi cookie trong cùng một site
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
            services.AddTransient<ICloudinaryService, CloudinaryService>();

            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProductService, ProductService>();
            
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProductDetailRepository, ProductDetailRepository>();
            services.AddTransient<IImageRepository, ImageRepository>();
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

                options.CustomSchemaIds(type => type.FullName);

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                
            });
        }
    }
}
