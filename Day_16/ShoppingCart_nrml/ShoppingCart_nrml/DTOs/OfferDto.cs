using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace ShoppingCartAPI.DTOs
{
    public class OfferDto
    {
        public decimal DiscountPercentage { get; set; }
        public string CouponCode { get; set; }
    }
}

