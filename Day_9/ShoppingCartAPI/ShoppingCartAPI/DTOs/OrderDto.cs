namespace ShoppingCartAPI.DTOs
{
    public class CheckoutDto
    {
        public string ShippingAddress { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItemResponseDto> Items { get; set; }
    }

    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
