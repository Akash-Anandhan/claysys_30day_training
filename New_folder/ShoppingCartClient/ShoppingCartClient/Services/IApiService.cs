using ShoppingCartClient.ViewModels;

public interface IApiService
{
    Task<AuthResponseViewModel> LoginAsync(LoginViewModel model);
    Task<string> RegisterAsync(RegisterViewModel model);
}
