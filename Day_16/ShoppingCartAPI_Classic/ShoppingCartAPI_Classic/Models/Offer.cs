namespace ShoppingCartAPI.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string CouponCode { get; set; }
        public bool IsActive { get; set; }
    }
}
