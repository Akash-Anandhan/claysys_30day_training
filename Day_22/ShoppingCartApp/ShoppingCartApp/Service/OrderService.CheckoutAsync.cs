// Services/OrderService.CheckoutAsync.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Order;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class OrderService
    {
        public async Task<ServiceResponse> CheckoutAsync(CheckoutDto dto)
        {
            try
            {
                var cartItems = await _context.CartItems.Include(c => c.Product).Where(c => c.UserId == dto.UserId).ToListAsync();
                if (!cartItems.Any())
                    return ServiceResponse.Redirect("Index", "Cart");
                // Validate stock for every item — collect all errors at once
                // so the user sees everything wrong in one go, not one at a time
                var stockErrors = cartItems.Where(c => c.Quantity > c.Product.Stock).Select(c => $"{c.Product.Name} has only {c.Product.Stock} items available").ToList();
                if (stockErrors.Any())
                    return new ServiceResponse
                    {
                        Succeeded = true,
                        RedirectAction = "Index",
                        RedirectController = "Cart",
                        TempData = new Dictionary<string, string>
                        {
                            {
                                "Error",
                                string.Join(", ", stockErrors)
                            }
                        }
                    };
                var user = await _userManager.FindByIdAsync(dto.UserId);
                
                // Parse user's saved address into separate fields
                var addressParts = ParseAddressParts(user?.Address);
                
                var checkoutPage = new CheckoutPageDto
                {
                    UserFullName = user?.FullName,
                    StreetAddress = addressParts.StreetAddress,
                    City = addressParts.City,
                    State = addressParts.State,
                    PostalCode = addressParts.PostalCode,
                    Country = addressParts.Country,
                    PhoneNumber = user?.PhoneNumber,
                    Items = cartItems.Select(c => new CheckoutItemDto { ProductId = c.ProductId, ProductName = c.Product.Name, ImageUrl = c.Product.ImageUrl, UnitPrice = c.SellingPrice, Quantity = c.Quantity, Stock = c.Product.Stock, Subtotal = c.SellingPrice * c.Quantity }).ToList()
                };
                return ServiceResponse.ShowView("Checkout", checkoutPage);
            }
            catch (Exception)
            {
                return ServiceResponse.ShowView("Error");
            }
        }
        
        // Helper method to parse composite address string into parts
        private (string StreetAddress, string City, string State, string PostalCode, string Country) ParseAddressParts(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return (string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            
            var parts = address.Split(',').Select(p => p.Trim()).ToArray();
            
            string streetAddress = parts.Length > 0 ? parts[0] : string.Empty;
            string city = parts.Length > 1 ? parts[1] : string.Empty;
            string stateAndPostal = parts.Length > 2 ? parts[2] : string.Empty;
            string country = parts.Length > 3 ? parts[3] : string.Empty;
            
            // Try to split state and postal code (e.g., "NY 10001")
            string state = string.Empty;
            string postalCode = string.Empty;
            
            if (!string.IsNullOrWhiteSpace(stateAndPostal))
            {
                var statePostalParts = stateAndPostal.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (statePostalParts.Length > 0) state = statePostalParts[0];
                if (statePostalParts.Length > 1) postalCode = statePostalParts[1];
            }
            
            return (streetAddress, city, state, postalCode, country);
        }
    }
}