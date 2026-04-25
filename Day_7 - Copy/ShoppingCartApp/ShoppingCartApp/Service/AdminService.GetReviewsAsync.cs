// Services/AdminService.GetReviewsAsync.cs
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
        // ── Reviews ────────────────────────────────────────────────────────────
        public async Task<ServiceResponse> GetReviewsAsync(
            string searchQuery = null,
            int? minRating = null,
            string sortBy = null,
            int page = 1,
            int pageSize = 50)
        {
            var query = _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .AsQueryable();

            // Search by product name or user email/name
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchLower = searchQuery.ToLower();
                query = query.Where(r => 
                    (r.Product != null && r.Product.Name.ToLower().Contains(searchLower)) ||
                    (r.User != null && (r.User.Email.ToLower().Contains(searchLower) || 
                       (r.User.FullName != null && r.User.FullName.ToLower().Contains(searchLower)))) ||
                    r.Comment.ToLower().Contains(searchLower));
            }

            // Filter by minimum rating
            if (minRating.HasValue && minRating.Value > 0)
            {
                query = query.Where(r => r.Rating >= minRating.Value);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy switch
            {
                "oldest" => query.OrderBy(r => r.CreatedAt),
                "rating_high" => query.OrderByDescending(r => r.Rating),
                "rating_low" => query.OrderBy(r => r.Rating),
                _ => query.OrderByDescending(r => r.CreatedAt) // newest (default)
            };

            // Apply pagination
            var reviews = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new AdminReviewsResultDto
            {
                Reviews = reviews,
                TotalCount = totalCount,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return ServiceResponse.ShowView("Reviews", result);
        }
    }
}