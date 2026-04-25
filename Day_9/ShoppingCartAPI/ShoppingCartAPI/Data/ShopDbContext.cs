using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Data
{
    public class ShopDbContext : IdentityDbContext<ApplicationUser>
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Offer> Offers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Review>().HasData(
                new Review { Id = 1, ProductId = 1, Comment = "Great phone!", Rating = 5 },
                new Review { Id = 2, ProductId = 1, Comment = "Battery life is okay.", Rating = 4 },
                new Review { Id = 3, ProductId = 2, Comment = "Nice laptop for the price.", Rating = 4 },
                new Review { Id = 4, ProductId = 3, Comment = "Sound quality is amazing.", Rating = 5 }
            );

            builder.Entity<Offer>().HasData(
                new Offer { Id = 1, ProductId = 1, DiscountPercentage = 10m, CouponCode = null, IsActive = true },
                new Offer { Id = 2, ProductId = 1, DiscountPercentage = 20m, CouponCode = "SAVE20", IsActive = true },
                new Offer { Id = 3, ProductId = 2, DiscountPercentage = 5m, CouponCode = null, IsActive = true },
                new Offer { Id = 4, ProductId = 3, DiscountPercentage = 15m, CouponCode = "AUDIO15", IsActive = true },
                new Offer { Id = 5, ProductId = 3, DiscountPercentage = 50m, CouponCode = "EXPIRED50", IsActive = false }
            );
        }
    }
}
