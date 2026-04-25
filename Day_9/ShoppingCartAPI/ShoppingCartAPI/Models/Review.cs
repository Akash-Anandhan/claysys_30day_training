namespace ShoppingCartAPI.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Comment { get; set; } = null!;
        public int Rating { get; set; }
    }
}
