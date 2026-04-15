using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCartAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public string ShippingAddress { get; set; }
        
        public string? PaymentType { get; set; }
        public string? PaymentId { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
