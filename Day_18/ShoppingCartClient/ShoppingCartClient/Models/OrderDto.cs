using System.Collections.Generic;

namespace ShoppingCartClient.Models
{
    public class CheckoutDto
    {
        public string? ShippingAddress { get; set; }
        public string? PaymentType { get; set; }
        public string? PaymentId { get; set; }
        public string? TransactionId { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string? PaymentType { get; set; }
        public string? PaymentId { get; set; }
        public string? TransactionId { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();
    }

    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
