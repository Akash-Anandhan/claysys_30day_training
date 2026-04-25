using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCartAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }  // "Pending", "Shipped", "Delivered"
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}