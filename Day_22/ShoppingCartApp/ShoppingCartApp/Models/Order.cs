using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCartApp.Models
{
    // Mark as partial so C# knows
    // there are more files for this class
    public partial class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public string ShippingAddress { get; set; }

        public string PhoneNumber { get; set; }

        public string PaymentMethod { get; set; } = "COD";

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}