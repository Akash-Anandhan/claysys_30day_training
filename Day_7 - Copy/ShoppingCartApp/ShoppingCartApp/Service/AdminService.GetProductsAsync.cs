// Services/AdminService.cs
using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ShoppingCartApp.DTOs.Admin;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;
using System.Globalization;
using System.Linq;

namespace ShoppingCartApp.Services
{
    public partial class AdminService
    {
        // ── Products ───────────────────────────────────────────────────────────
        public async Task<ServiceResponse> GetProductsAsync(
            string searchQuery = null,
            string category = null,
            string stockFilter = null,
            string sortBy = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 50)
        {
            var query = _context.Products.Include(p => p.Category).Include(p => p.Reviews).AsQueryable();

            // Search by name or description
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchLower = searchQuery.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower) || 
                                        (p.Description != null && p.Description.ToLower().Contains(searchLower)));
            }

            // Filter by category
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name == category);
            }

            // Filter by stock status
            if (!string.IsNullOrWhiteSpace(stockFilter))
            {
                switch (stockFilter)
                {
                    case "in_stock":
                        query = query.Where(p => p.Stock >= 10);
                        break;
                    case "low_stock":
                        query = query.Where(p => p.Stock > 0 && p.Stock < 10);
                        break;
                    case "out_of_stock":
                        query = query.Where(p => p.Stock == 0);
                        break;
                }
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.SellingPrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.SellingPrice <= maxPrice.Value);
            }

            // Get total count before sorting/pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy switch
            {
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "price_asc" => query.OrderBy(p => p.SellingPrice),
                "price_desc" => query.OrderByDescending(p => p.SellingPrice),
                "stock_asc" => query.OrderBy(p => p.Stock),
                "stock_desc" => query.OrderByDescending(p => p.Stock),
                "newest" => query.OrderByDescending(p => p.Id),
                _ => query.OrderByDescending(p => p.Id)
            };

            // Apply pagination
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            var viewModel = new AdminProductsViewModel
            {
                SearchQuery = searchQuery,
                Category = category,
                StockFilter = stockFilter,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                Products = products,
                Categories = categories,
                TotalResults = products.Count,
                TotalProducts = totalCount
            };

            return ServiceResponse.ShowView("Products", viewModel);
        }
    }
}