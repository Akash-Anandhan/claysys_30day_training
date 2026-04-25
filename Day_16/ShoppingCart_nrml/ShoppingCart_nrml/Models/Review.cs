using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace ShoppingCartAPI.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}

