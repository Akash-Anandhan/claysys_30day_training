using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class AdminService
    {
        public async Task<ServiceResponse> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return ServiceResponse.ShowView("Users", users);
        }

        public async Task<ServiceResponse> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return ServiceResponse.Redirect("Users", "Admin", new Dictionary<string, string> { { "Success", "User deleted." } });
        }
    }
}