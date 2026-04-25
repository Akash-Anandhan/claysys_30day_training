namespace ShoppingCartClient.ViewModels
{
    public class AuthResponseViewModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Expiration { get; set; }
    }
}