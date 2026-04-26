// DTOs/Order/OrderDtos.cs
namespace ShoppingCartApp.DTOs.Order
{
    // ── Inbound DTOs (Controller → Service) ───────────────────────────────

    public class CheckoutDto
    {
        public string UserId { get; set; }  // resolved from claims by controller
    }

    public class PlaceOrderDto
    {
        public string UserId { get; set; }  // resolved from claims by controller
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class OrderConfirmationDto
    {
        public string UserId { get; set; }  // resolved from claims by controller
        public int OrderId { get; set; }
    }

    // ── Outbound DTOs (Service → Controller) ──────────────────────────────

    public class CheckoutPageDto
    {
        public List<CheckoutItemDto> Items { get; set; } = new();
        public string UserFullName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class CheckoutItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class OrderConfirmationPageDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}