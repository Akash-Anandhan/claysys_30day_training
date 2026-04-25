// DTOs/Cart/CartDtos.cs
namespace ShoppingCartApp.DTOs.Cart
{
    // ── Inbound DTOs (Controller → Service) ───────────────────────────────

    public class AddToCartDto
    {
        public string UserId { get; set; }  // resolved by controller from session/claims
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public bool IsAjax { get; set; }  // controller reads request header, passes it in
    }

    public class RemoveFromCartDto
    {
        public string UserId { get; set; }
        public int ItemId { get; set; }
    }

    public class UpdateQuantityDto
    {
        public string UserId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class MergeCartDto
    {
        public string GuestId { get; set; }
        public string UserId { get; set; }
    }

    // ── Outbound DTOs (Service → Controller) ──────────────────────────────

    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Total { get; set; }
        public int TotalCount { get; set; }
    }

    public class UpdateQuantityResultDto
    {
        public bool Success { get; set; }
        public bool Removed { get; set; }
        public string Message { get; set; }
        public string Subtotal { get; set; }
        public string CartTotal { get; set; }
        public int CartCount { get; set; }
    }
}