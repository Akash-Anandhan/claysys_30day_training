using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCartApp.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}