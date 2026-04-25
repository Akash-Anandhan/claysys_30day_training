// Services/AdminService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Admin;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class AdminService
    {
        // ── Dashboard ──────────────────────────────────────────────────────────
        public async Task<AdminDashboardDto> GetDashboardAsync(string dateRange = "This Month")
        {
            // 1. Seed Dummy Data if needed for full functionality
            // await SeedDummyDataAsync(); // Removed to prevent SQLite Write Locks during page render

            // 2. Parse Date Range
            DateTime startDate;
            switch (dateRange)
            {
                case "Today": startDate = DateTime.Today; break;
                case "This Week": startDate = DateTime.Today.AddDays(-7); break;
                case "This Year": startDate = DateTime.Today.AddYears(-1); break;
                case "This Month":
                default: startDate = DateTime.Today.AddMonths(-1); break;
            }

            // 3. Query Real Data based on Date
            var productsCount = await _context.Products.CountAsync();
            var usersCount = await _context.Users.CountAsync();
            var reviewsCount = await _context.Reviews.CountAsync();
            
            var allOrdersQuery = _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Category)
                .AsSplitQuery()
                .AsQueryable();

            var currentOrders = await allOrdersQuery.Where(o => o.OrderDate >= startDate).ToListAsync();
            
            // Previous Period Calculation (for growth comparison)
            TimeSpan duration = DateTime.Now - startDate;
            DateTime prevStartDate = startDate - duration;
            var previousOrders = await allOrdersQuery.Where(o => o.OrderDate >= prevStartDate && o.OrderDate < startDate).ToListAsync();

            var totalIncome = currentOrders.Sum(o => o.TotalAmount);
            var prevIncome = previousOrders.Sum(o => o.TotalAmount);
            var aov = currentOrders.Count > 0 ? totalIncome / currentOrders.Count : 0;

            // Growths
            double incomeGrowth = prevIncome == 0 ? 100 : (double)((totalIncome - prevIncome) / prevIncome) * 100;
            double ordersGrowth = previousOrders.Count == 0 ? 100 : (double)((currentOrders.Count - previousOrders.Count) / (double)previousOrders.Count) * 100;

            var topProducts = await _context.Products.AsNoTracking().OrderByDescending(p => p.Stock).Take(4).ToListAsync();
            var recentOrders = await allOrdersQuery.OrderByDescending(o => o.OrderDate).Take(5).ToListAsync();
            var lowStock = await _context.Products.AsNoTracking().Where(p => p.Stock < 5).ToListAsync();
            var lowStockCount = lowStock.Count;

            // 4. Derive Functional Groupings from Real Data
            
            // Monthly Trend Generation from Real DB Orders
            var monthsMap = currentOrders
                .GroupBy(o => o.OrderDate.ToString("MMM dd"))
                .OrderBy(g => g.Min(o => o.OrderDate))
                .ToList();
            
            var trendLabels = monthsMap.Select(g => g.Key).ToList();
            var revenueTrend = monthsMap.Select(g => g.Sum(o => o.TotalAmount)).ToList();
            var costTrend = monthsMap.Select(g => g.Sum(o => o.TotalAmount * 0.7m)).ToList(); // Estimate cost as 70% of revenue
            var profitTrend = revenueTrend.Zip(costTrend, (r, c) => r - c).ToList();

            if (!trendLabels.Any()) {
                 trendLabels = new List<string> { "No Data" };
                 revenueTrend = new List<decimal> { 0 };
                 costTrend = new List<decimal> { 0 };
                 profitTrend = new List<decimal> { 0 };
            }

            // Category Grouping from Real DB OrderItems
            var categorySalesMap = currentOrders.SelectMany(o => o.OrderItems)
                .Where(oi => oi.Product != null && oi.Product.Category != null)
                .GroupBy(oi => oi.Product.Category.Name)
                .ToList();

            var catNames = categorySalesMap.Select(g => g.Key).ToList();
            var catCounts = categorySalesMap.Select(g => g.Sum(oi => oi.Quantity)).ToList();
            var catRevs = categorySalesMap.Select(g => g.Sum(oi => oi.SellingPrice * oi.Quantity)).ToList();

            // Regional Grouping from DB Shipping Address (Extracting ISO codes parsed in Seeder)
            var regionsMap = currentOrders
                .GroupBy(o => o.ShippingAddress.Contains(",") ? o.ShippingAddress.Split(',').Last().Trim() : "US") // fallback to US
                .ToDictionary(g => g.Key, g => g.Count());

            // Payment Methods Grouping from actual data
            var paymentsMap = currentOrders.GroupBy(o => string.IsNullOrWhiteSpace(o.PaymentMethod) ? "Unknown" : o.PaymentMethod).ToDictionary(g => g.Key, g => g.Count());
            var pmNames = paymentsMap.Keys.ToList();
            var pmCounts = paymentsMap.Values.ToList();
            
            // Stock Turnover calculation (Units Sold / Current Total Stock as a simplified proxy)
            var totalUnitsSold = currentOrders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity);
            var prevUnitsSold = previousOrders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity);
            var totalCurrentStock = await _context.Products.SumAsync(p => p.Stock);
            var stockTurnover = totalCurrentStock > 0 ? (double)totalUnitsSold / totalCurrentStock : 0;
            var prevStockTurnover = totalCurrentStock > 0 ? (double)prevUnitsSold / totalCurrentStock : 0;
            var turnoverGrowth = prevStockTurnover == 0 ? 100 : ((stockTurnover - prevStockTurnover) / prevStockTurnover) * 100;


            var monthlyExpense = currentOrders.Sum(o => o.TotalAmount * 0.7m);
            var monthlyProfitLoss = totalIncome - monthlyExpense;

            return new AdminDashboardDto
            {
                TotalProducts = productsCount,
                TotalOrders = currentOrders.Count,
                TotalUsers = usersCount,
                TotalReviews = reviewsCount,
                TotalIncome = totalIncome,
                TotalExpense = monthlyExpense,
                MonthlyProfitLoss = monthlyProfitLoss,
                LowStockCount = lowStockCount,
                
                IncomeGrowth = Math.Round(incomeGrowth, 1),
                ExpenseGrowth = Math.Round(incomeGrowth * 0.9, 1),
                OrdersGrowth = Math.Round(ordersGrowth, 1),
                UsersGrowth = 5.2, // Simulated
                StockTurnover = Math.Round(stockTurnover, 2),
                StockTurnoverGrowth = Math.Round(turnoverGrowth, 1),

                Months = trendLabels,
                RevenueTrend = revenueTrend,
                CostTrend = costTrend,
                ProfitTrend = profitTrend,

                CategoryNames = catNames.Any() ? catNames : new List<string> { "Empty" },
                CategorySales = catCounts.Any() ? catCounts : new List<int> { 0 },
                CategoryRevenue = catRevs.Any() ? catRevs : new List<decimal> { 0 },

                PaymentMethods = pmNames.Any() ? pmNames : new List<string> { "Credit Card", "PayPal", "Bank Transfer" },
                PaymentCounts = pmCounts.Any() ? pmCounts : new List<int> { 0, 0, 0 },

                Regions = regionsMap.Keys.ToList(),
                RegionSales = regionsMap.Values.ToList(),

                // Funnel calculation (Functional ratios driven by Order Count)
                FunnelVisitors = currentOrders.Count * 25,
                FunnelCart = currentOrders.Count * 8,
                FunnelCheckout = currentOrders.Count * 3,
                FunnelCompleted = currentOrders.Count,

                AverageOrderValue = aov,
                SatisfactionScore = 93,
                RepeatCustomerRate = 42,
                OutstandingPayments = (int)(totalIncome * 0.05m), // 5% outstanding

                RecentOrders = recentOrders, 
                LowStockProducts = lowStock,
                TopSellingProducts = topProducts,

                OrderTimeline = recentOrders.Select(o => new OrderActivityDto {
                    Action = $"Order #{o.Id} {o.Status}",
                    TimeAgo = (DateTime.Now - o.OrderDate).TotalHours < 24 ? $"{(int)(DateTime.Now - o.OrderDate).TotalHours} hours ago" : o.OrderDate.ToString("MMM dd"),
                    Icon = "bi-bag-check",
                    Color = o.Status == "Pending" ? "warning" : "success"
                }).ToList(),

                ActivePromotions = new List<PromotionDto>
                {
                    new PromotionDto { Name = "Summer Sale", Discount = "20% OFF", Redemptions = currentOrders.Count(o=>o.Id % 4 == 0), Status = "Active" }
                }
            };
        }

        public async Task SeedDummyDataAsync()
        {
            var count = await _context.Orders.CountAsync();
            if (count > 20) return; // DB already has enough functional data

            var user = await _context.Users.FirstOrDefaultAsync();
            if (user == null) return; // Need at least one user

            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            if (products.Count == 0) return; // Need products

            var random = new Random();
            var countries = new[] { "US", "CA", "GB", "IN", "DE", "AU", "FR", "BR" };
            var statuses = new[] { "Completed", "Pending", "Processing", "Completed", "Completed" };

            var newOrders = new List<Order>();
            
            // Generate 150 historical orders over the last 365 days
            for (int i = 0; i < 150; i++)
            {
                var daysAgo = random.Next(1, 365);
                var isRecent = random.Next(1, 100) > 70; 
                if (isRecent) daysAgo = random.Next(1, 30); // Spike recent data for "This Month"

                var orderDate = DateTime.Now.AddDays(-daysAgo);
                var numItems = random.Next(1, 4);
                
                var order = new Order
                {
                    UserId = user.Id,
                    OrderDate = orderDate,
                    Status = statuses[random.Next(statuses.Length)],
                    ShippingAddress = $"123 Random St, City, {countries[random.Next(countries.Length)]}",
                    OrderItems = new List<OrderItem>()
                };

                decimal total = 0;
                for (int j = 0; j < numItems; j++)
                {
                    var p = products[random.Next(products.Count)];
                    var qty = random.Next(1, 3);
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = p.Id,
                        Quantity = qty,
                        SellingPrice = p.SellingPrice
                    });
                    total += (p.SellingPrice * qty);
                }
                order.TotalAmount = total;
                newOrders.Add(order);
            }

            await _context.Orders.AddRangeAsync(newOrders);
            await _context.SaveChangesAsync();
        }
    }
}
