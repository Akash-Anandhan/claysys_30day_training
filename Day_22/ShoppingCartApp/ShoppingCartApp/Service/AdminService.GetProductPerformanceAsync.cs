// Services/AdminService.GetProductPerformanceAsync.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartApp.Services
{
    public partial class AdminService
    {
        public async Task<ProductPerformanceViewModel> GetProductPerformanceAsync(
            string searchQuery = null,
            string category = null,
            string sortBy = null,
            int page = 1,
            int pageSize = 20)
        {
            // Get all products with AsNoTracking
            IQueryable<Product> productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .AsNoTracking();

            // Apply filters
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var searchLower = searchQuery.ToLower();
                productsQuery = productsQuery.Where(p => 
                    p.Name.ToLower().Contains(searchLower) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchLower)));
            }

            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category != null && p.Category.Name == category);
            }

            var products = await productsQuery.ToListAsync();
            
            // Get all order items for these products
            var productIds = products.Select(p => p.Id).ToList();
            var allOrderItems = await _context.OrderItems
                .Where(oi => productIds.Contains(oi.ProductId))
                .Include(oi => oi.Order)
                .AsNoTracking()
                .ToListAsync();

            // Calculate performance metrics for each product
            var productPerformanceList = new List<ProductPerformanceItem>();

            foreach (var product in products)
            {
                // Get order items for this product
                var orderItems = allOrderItems.Where(oi => oi.ProductId == product.Id).ToList();
                var orders = orderItems.Where(oi => oi.Order != null).Select(oi => oi.Order).ToList();
                var uniqueOrders = orders.GroupBy(o => o.Id).Select(g => g.First()).ToList();

                // Calculate country distribution from unique orders
                var countryDistribution = new Dictionary<string, int>();
                foreach (var order in orders)
                {
                    var country = ExtractCountryFromAddress(order.ShippingAddress);
                    if (!string.IsNullOrEmpty(country))
                    {
                        if (!countryDistribution.ContainsKey(country))
                            countryDistribution[country] = 0;
                        countryDistribution[country]++;
                    }
                }

                var totalUnits = orderItems.Sum(oi => oi.Quantity);
                var totalRevenue = orderItems.Sum(oi => oi.SellingPrice * oi.Quantity);
                var avgRating = product.Reviews?.Any() == true ? product.Reviews.Average(r => r.Rating) : 0;

                var performanceItem = new ProductPerformanceItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    CategoryName = product.Category?.Name ?? "Uncategorized",
                    TotalOrders = uniqueOrders.Count,
                    TotalUnitsSold = totalUnits,
                    TotalRevenue = totalRevenue,
                    AverageOrderValue = uniqueOrders.Count > 0 ? totalRevenue / uniqueOrders.Count : 0,
                    TotalReviews = product.Reviews?.Count ?? 0,
                    AverageRating = avgRating,
                    CountryDistribution = countryDistribution,
                    TopCountry = countryDistribution.OrderByDescending(c => c.Value).FirstOrDefault().Key ?? "N/A",
                    Price = product.SellingPrice,
                    Stock = product.Stock
                };

                productPerformanceList.Add(performanceItem);
            }

            // Apply sorting
            switch (sortBy)
            {
                case "revenue_desc":
                    productPerformanceList = productPerformanceList.OrderByDescending(p => p.TotalRevenue).ToList();
                    break;
                case "revenue_asc":
                    productPerformanceList = productPerformanceList.OrderBy(p => p.TotalRevenue).ToList();
                    break;
                case "orders_desc":
                    productPerformanceList = productPerformanceList.OrderByDescending(p => p.TotalOrders).ToList();
                    break;
                case "orders_asc":
                    productPerformanceList = productPerformanceList.OrderBy(p => p.TotalOrders).ToList();
                    break;
                case "rating_desc":
                    productPerformanceList = productPerformanceList.OrderByDescending(p => p.AverageRating).ToList();
                    break;
                case "rating_asc":
                    productPerformanceList = productPerformanceList.OrderBy(p => p.AverageRating).ToList();
                    break;
                case "name_asc":
                    productPerformanceList = productPerformanceList.OrderBy(p => p.ProductName).ToList();
                    break;
                case "name_desc":
                    productPerformanceList = productPerformanceList.OrderByDescending(p => p.ProductName).ToList();
                    break;
                default:
                    productPerformanceList = productPerformanceList.OrderByDescending(p => p.TotalRevenue).ToList();
                    break;
            }

            // Pagination
            var totalCount = productPerformanceList.Count;
            var totalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);
            var paginatedProducts = productPerformanceList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Get categories for filter dropdown
            var categories = await _context.Categories.AsNoTracking().ToListAsync();

            return new ProductPerformanceViewModel
            {
                Products = paginatedProducts,
                TotalProducts = totalCount,
                TotalRevenue = productPerformanceList.Sum(p => p.TotalRevenue),
                TotalUnitsSold = productPerformanceList.Sum(p => p.TotalUnitsSold),
                TotalReviews = productPerformanceList.Sum(p => p.TotalReviews),
                SearchQuery = searchQuery,
                Category = category,
                SortBy = sortBy,
                CurrentPage = page,
                TotalPages = totalPages,
                Categories = categories
            };
        }

        // Helper method to extract country from shipping address
        private string ExtractCountryFromAddress(string shippingAddress)
        {
            if (string.IsNullOrEmpty(shippingAddress))
                return "Unknown";

            // Try to get the last part after comma (e.g., "123 Random St, City, IN" -> "IN")
            var parts = shippingAddress.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                var lastPart = parts.Last().Trim().ToUpper();

                // Common country codes or abbreviations
                if (lastPart == "IN" || lastPart == "INDIA") return "India";
                if (lastPart == "US" || lastPart == "USA" || lastPart == "UNITED STATES") return "USA";
                if (lastPart == "UK" || lastPart == "UNITED KINGDOM") return "UK";
                if (lastPart == "CA" || lastPart == "CANADA") return "Canada";
                if (lastPart == "AU" || lastPart == "AUSTRALIA") return "Australia";

                // If lastPart is a 2-letter code not handled, maybe it's a state, but we'll treat it as a region
                if (lastPart.Length == 2)
                {
                    // For the purpose of this demo, if it's 2 letters and we don't know it, let's assume India if the address "feels" like it, or just return the code
                    return lastPart;
                }
            }

            // Fallback to keyword search
            var countries = new[] { 
                "USA", "United States", "UK", "United Kingdom", "India", "Canada", 
                "Australia", "Germany", "France", "Japan", "China", "Brazil", "Mexico",
                "Italy", "Spain", "Netherlands", "Switzerland", "Sweden", "Poland", "Turkey",
                "Russia", "South Korea", "Indonesia", "Saudi Arabia", "UAE", "United Arab Emirates",
                "South Africa", "Nigeria", "Egypt", "Argentina", "Colombia", "Chile", "Peru",
                "New Zealand", "Singapore", "Malaysia", "Thailand", "Vietnam", "Philippines"
            };

            foreach (var country in countries)
            {
                if (shippingAddress.Contains(country, System.StringComparison.OrdinalIgnoreCase))
                    return country;
            }

            return "Other";
        }
    }
}