using ShoppingCartAPI.Models;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Services
{
    public partial class OrdersService
    {
        private string ValidateAndSetShippingAddressAsync(ApplicationUser user, string providedAddress)
        {
            string finalAddress = providedAddress;

            if (string.IsNullOrWhiteSpace(finalAddress))
            {
                if (string.IsNullOrWhiteSpace(user.Address))
                {
                    throw new ArgumentException("Shipping Address is required for the first checkout.");
                }
                finalAddress = user.Address;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(user.Address))
                {
                    user.Address = finalAddress;
                    _context.Users.Update(user);
                }
            }

            return finalAddress;
        }
    }
}
