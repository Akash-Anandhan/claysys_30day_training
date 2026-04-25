// DTOs/Wishlist/WishlistDtos.cs
namespace ShoppingCartApp.DTOs.Wishlist
{
    // ── Inbound DTOs (Controller → Service) ───────────────────────────────

    public class AddToWishlistDto
    {
        public string UserId { get; set; }  // resolved from claims by controller
        public int ProductId { get; set; }
    }

    public class RemoveFromWishlistDto
    {
        public string UserId { get; set; }
        public int ItemId { get; set; }
    }

    public class MoveToCartDto
    {
        public string UserId { get; set; }
        public int ItemId { get; set; }
    }

    // ── Outbound DTOs (Service → Controller) ──────────────────────────────

    public class WishlistItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public DateTime AddedOn { get; set; }
    }
}