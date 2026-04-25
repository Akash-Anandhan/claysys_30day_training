using Microsoft.AspNet.Identity.EntityFramework;
using ShoppingCartAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace ShoppingCartAPI.Data
{
    public class ShopDbContext : IdentityDbContext<ApplicationUser>
    {
        public ShopDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ShopDbContext Create()
        {
            return new ShopDbContext();
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Offer> Offers { get; set; }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seeding data is tricky in general OnModelCreating in EF6 without Migrations Seed method,
            // but for simplicity, we keep it as it was if possible, or omit since it's just dummy data.
            // EF6 doesn't have HasData. We have to use Migrations Configuration.cs for seeding.
            // However, to keep it structurally similar, we just define mapping configurations here.
        }
    }
}
