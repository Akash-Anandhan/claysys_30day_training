using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Services
{
    public partial class WishlistService
    {
        private async Task<bool> ProductExistsAsync(int productId)
        {
            return await _context.Products.AnyAsync(p => p.Id == productId);
        }
    }
}
