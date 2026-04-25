namespace ShoppingCartClient.Models
{
    public class WishlistItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class AddWishlistDto
    {
        public int ProductId { get; set; }
    }
}
