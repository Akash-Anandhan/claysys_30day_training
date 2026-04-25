// Services/OrderService.cs
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
                var checkoutPage = new CheckoutPageDto
                {
                    UserFullName = user?.FullName,
                    UserAddress = user?.Address,
                    Items = cartItems.Select(c => new CheckoutItemDto { ProductId = c.ProductId, ProductName = c.Product.Name, ImageUrl = c.Product.ImageUrl, UnitPrice = c.SellingPrice, Quantity = c.Quantity, Stock = c.Product.Stock, Subtotal = c.SellingPrice * c.Quantity }).ToList()
                };
                return ServiceResponse.ShowView("Checkout", checkoutPage);
            }
            catch (Exception)
            {
                return ServiceResponse.ShowView("Error");
            }
        }
    }
}