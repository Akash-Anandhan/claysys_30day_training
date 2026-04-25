using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShoppingCartClient.Models;

namespace ShoppingCartClient.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }
            return null;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/products");
            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
                return products ?? new List<ProductDto>();
            }
            return new List<ProductDto>();
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductDto>();
            }
            return null;
        }

        public async Task<bool> UpdateProductAsync(ProductDto productDto)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/products/{productDto.Id}", productDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddToCartAsync(AddToCartDto addToCartDto)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/cart", addToCartDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveFromCartAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/cart/{id}");
            return response.IsSuccessStatusCode;
        }

        public class UpdateCartResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public async Task<UpdateCartResponse> UpdateCartItemAsync(int id, UpdateCartDto dto)
        {
            AddAuthorizationHeader();

            var response = await _httpClient.PutAsJsonAsync($"api/cart/update/{id}", dto);

            if (response.IsSuccessStatusCode)
            {
                return new UpdateCartResponse { Success = true };
            }

            var error = await response.Content.ReadAsStringAsync();

            try
            {
                var errorObj = System.Text.Json.JsonSerializer.Deserialize<UpdateCartResponse>(error);
                return errorObj ?? new UpdateCartResponse { Success = false, Message = "Update failed" };
            }
            catch
            {
                return new UpdateCartResponse { Success = false, Message = error };
            }
        }

        public async Task<CartDto?> GetCartAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/cart");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CartDto>();
            }
            return null;
        }

        public async Task<List<WishlistItemDto>> GetWishlistAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/wishlist");
            if (response.IsSuccessStatusCode)
            {
                var dict = await response.Content.ReadFromJsonAsync<List<WishlistItemDto>>();
                return dict ?? new List<WishlistItemDto>();
            }
            return new List<WishlistItemDto>();
        }

        public async Task<bool> AddToWishlistAsync(AddWishlistDto addWishlistDto)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync("api/wishlist", addWishlistDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveFromWishlistAsync(int productId)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/wishlist/{productId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<int> CheckoutAsync(CheckoutDto checkoutDto)
        {
            AddAuthorizationHeader();

            var response = await _httpClient.PostAsJsonAsync("api/orders/checkout", checkoutDto);

           
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CheckoutResult>();
                return result?.OrderId ?? 0;
            }

            // ❗ Read actual error message from API
            var errorContent = await response.Content.ReadAsStringAsync();

            // Try to parse structured error (if you used DTO)
            try
            {
                var errorObj = System.Text.Json.JsonSerializer.Deserialize<CheckoutResult>(errorContent);
                if (!string.IsNullOrEmpty(errorObj?.Message))
                {
                    throw new Exception(errorObj.Message);
                }
            }
            catch
            {
                // fallback if parsing fails
            }

            // fallback generic error
            throw new Exception("Checkout failed. Please try again.");
        }

        public class CheckoutResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public int OrderId { get; set; }
        }

        public async Task<List<UserProfileDto>> GetUsersAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/auth/users");
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<UserProfileDto>>();
                return users ?? new List<UserProfileDto>();
            }
            return new List<UserProfileDto>();
        }

        public async Task<List<OrderResponseDto>> GetOrdersAsync()
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync("api/orders");
            if (response.IsSuccessStatusCode)
            {
                var orders = await response.Content.ReadFromJsonAsync<List<OrderResponseDto>>();
                return orders ?? new List<OrderResponseDto>();
            }
            return new List<OrderResponseDto>();
        }

        public async Task<byte[]> DownloadOrdersExportAsync(string format)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"api/orders/export/{format}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }

        public async Task<byte[]> DownloadProductsExportAsync(string format)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"api/products/export/{format}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }

        public async Task<string> ImportProductsAsync(Microsoft.AspNetCore.Http.IFormFile file, string format)
        {
            AddAuthorizationHeader();
            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            using var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);
            
            var response = await _httpClient.PostAsync($"api/products/import/{format}", content);
            if (response.IsSuccessStatusCode)
            {
                // Reading string message
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            return "Failed to import products.";
        }
    }
}
