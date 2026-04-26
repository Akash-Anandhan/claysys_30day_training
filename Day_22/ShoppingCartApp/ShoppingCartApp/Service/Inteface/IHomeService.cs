// Services/Interface/IHomeService.cs
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Services
{
    public interface IHomeService
    {
        Task<HomeViewModel> GetHomeViewModelAsync();
    }
}