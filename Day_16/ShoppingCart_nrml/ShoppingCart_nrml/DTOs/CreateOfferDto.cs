using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace ShoppingCartAPI.DTOs
{
    public class CreateOfferDto
    {
        public decimal DiscountPercentage { get; set; }
        public string CouponCode { get; set; }
        public bool IsActive { get; set; }
    }
}

