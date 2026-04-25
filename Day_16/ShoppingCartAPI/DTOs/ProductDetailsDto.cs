using System.Collections.Generic;

namespace ShoppingCartAPI.DTOs
{
    public class ProductDetailsDto
    {
        public ProductDto Product { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
        public List<OfferDto> Offers { get; set; } = new List<OfferDto>();
        public int Stock { get; set; }
    }
}
