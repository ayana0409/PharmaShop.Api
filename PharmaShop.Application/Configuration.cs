﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Services;

namespace PharmaShop.Application
{
    public static class Configuration
    {
        public static void AddAppDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient<ICloudinaryService, CloudinaryService>();
            services.AddTransient<IPayPalService, PayPalService>();

            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IImportService, ImportService>();
            services.AddTransient<IShopService, ShopService>();
            services.AddTransient<ICartService, CartService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ITypeService, TypeService>();
            services.AddTransient<IReportService, ReportService>();
        }

        public static void AddPaymentMethod(this IServiceCollection services, IConfiguration configuration)
        {
        }
    }
}
