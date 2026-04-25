// DTOs/Auth/ApiAuthDtos.cs
namespace ShoppingCartApp.DTOs.Auth
{
    // ── Inbound DTOs (Controller → Service) ──

    public class ApiLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

    // ── Outbound DTO (Service → Controller) ──

    public class TokenResultDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
