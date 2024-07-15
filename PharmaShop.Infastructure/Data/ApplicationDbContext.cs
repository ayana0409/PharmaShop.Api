using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmaShop.Infastructure.Entities;
using PharmaShop.Infastructure.Models;
using System.Configuration;
using System.Drawing;
using System.Reflection.Emit;

namespace PharmaShop.Infastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base (options)
        {
            
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<UserAddress> UserAddress { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<ProductInventory> ProductInventorys { get; set; }
        public DbSet<Import> Imports { get; set; }
        public DbSet<ImportDetail> ImportDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserType>().HasData(
                new UserType
                {
                    Id = 1,
                    Name = "Bronze",
                    Discount = 0,
                    MaxDiscount = 0,
                    Point = 0,
                    IsActive = true
                }
            );
            //AspNetUser
            builder.Entity<ApplicationUser>().ToTable("ApplicationUser");
            builder.Entity<IdentityRole>().ToTable("ApplicationRole");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UseLogin");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");

            builder.Entity<ImportDetail>().HasKey(d => new
            {
                d.ProductId,
                d.ImportId
            });
            builder.Entity<OrderDetail>().HasKey(o => new
            {
                o.ProductId,
                o.OrderId
            });

        }
    }
}
