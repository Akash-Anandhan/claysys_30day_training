namespace ShoppingCartAPI.DTOs
{
    public class CreateOfferDto
    {
        public decimal DiscountPercentage { get; set; }
        public string CouponCode { get; set; }
        public bool IsActive { get; set; }
    }
}
