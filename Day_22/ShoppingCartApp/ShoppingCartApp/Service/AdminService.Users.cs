using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class AdminService
    {
        public async Task<ServiceResponse> GetUsersAsync(string searchQuery = null)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var lowerQuery = searchQuery.ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(lowerQuery) || u.Email.ToLower().Contains(lowerQuery));
            }

            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
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