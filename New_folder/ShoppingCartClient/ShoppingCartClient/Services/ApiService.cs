using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ShoppingCartClient.ViewModels;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private void AddJwtHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JWT");

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<AuthResponseViewModel> LoginAsync(LoginViewModel model)
    {
        var payload = new
        {
            email = model.Email,
            password = model.Password
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("api/Auth/login", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Invalid login");

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<AuthResponseViewModel>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<string> RegisterAsync(RegisterViewModel model)
    {
        var payload = new
        {
            email = model.Email,
            password = model.Password,
            fullName = model.FullName,
            phoneNumber = model.PhoneNumber,
            address = model.Address,
            role = model.Role
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("api/Auth/register", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Registration failed");

        return await response.Content.ReadAsStringAsync();
    }
}