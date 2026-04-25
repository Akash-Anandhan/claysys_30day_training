// Services/OrderService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Order;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderService(
            ShopDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ServiceResponse> CheckoutAsync(CheckoutDto dto)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Include(c => c.Product)
                    .Where(c => c.UserId == dto.UserId)
                    .ToListAsync();

                if (!cartItems.Any())
                    return ServiceResponse.Redirect("Index", "Cart");

                // Validate stock for every item — collect all errors at once
                // so the user sees everything wrong in one go, not one at a time
                var stockErrors = cartItems
                    .Where(c => c.Quantity > c.Product.Stock)
                    .Select(c => $"{c.Product.Name} has only {c.Product.Stock} items available")
                    .ToList();

                if (stockErrors.Any())
                    return new ServiceResponse
                    {
                        Succeeded = true,
                        RedirectAction = "Index",
                        RedirectController = "Cart",
                        TempData = new Dictionary<string, string>
                        {
                            { "Error", string.Join(", ", stockErrors) }
                        }
                    };

                var user = await _userManager.FindByIdAsync(dto.UserId);

                var checkoutPage = new CheckoutPageDto
                {
                    UserFullName = user?.FullName,
                    UserAddress = user?.Address,
                    Items = cartItems.Select(c => new CheckoutItemDto
                    {
                        ProductId = c.ProductId,
                        ProductName = c.Product.Name,
                        ImageUrl = c.Product.ImageUrl,
                        UnitPrice = c.UnitPrice,
                        Quantity = c.Quantity,
                        Stock = c.Product.Stock,
                        Subtotal = c.UnitPrice * c.Quantity
                    }).ToList()
                };

                return ServiceResponse.ShowView("Checkout", checkoutPage);
            }
            catch (Exception)
            {
                return ServiceResponse.ShowView("Error");
            }
        }

        public async Task<ServiceResponse> PlaceOrderAsync(PlaceOrderDto dto)
        {
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == dto.UserId)
                .ToListAsync();

            if (!cartItems.Any())
                return ServiceResponse.Redirect("Index", "Cart");

            // Stock check — fail fast on first violation
            var stockViolation = cartItems
                .FirstOrDefault(c => c.Quantity > c.Product.Stock);

            if (stockViolation != null)
                return new ServiceResponse
                {
                    Succeeded = true,
                    RedirectAction = "Index",
                    RedirectController = "Cart",
                    TempData = new Dictionary<string, string>
                    {
                        {
                            "Error",
                            $"Only {stockViolation.Product.Stock} items available " +
                            $"for {stockViolation.Product.Name}"
                        }
                    }
                };

            // Create order
            var order = new Order
            {
                UserId = dto.UserId,
                OrderDate = DateTime.Now,
                ShippingAddress = dto.ShippingAddress,
                Status = "Pending",
                TotalAmount = cartItems.Sum(c => c.UnitPrice * c.Quantity),
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice
                }).ToList()
            };

            // Reduce stock
            foreach (var item in cartItems)
                item.Product.Stock -= item.Quantity;

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return ServiceResponse.Redirect(
                "Confirmation",
                "Order",
                new { id = order.Id });
        }

        public async Task<ServiceResponse> GetConfirmationAsync(OrderConfirmationDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId
                                       && o.UserId == dto.UserId);

            if (order == null)
                return ServiceResponse.ShowView(
                    "NotFound", null, string.Empty, "Order not found.");

            var confirmationPage = new OrderConfirmationPageDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.Product.Name,
                    ImageUrl = oi.Product.ImageUrl,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.UnitPrice * oi.Quantity
                }).ToList()
            };

            return ServiceResponse.ShowView("Confirmation", confirmationPage);
        }
    }
}