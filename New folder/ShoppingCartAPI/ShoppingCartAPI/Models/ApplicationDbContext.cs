// Models/ApplicationDbContext.cs
using Microsoft.AspNet.Identity.EntityFramework;
using ShoppingCartAPI.Models;
using System.Data.Entity;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext() : base("DefaultConnection") { }

    public DbSet<Product> Products { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }

    public static ApplicationDbContext Create() => new ApplicationDbContext();
}