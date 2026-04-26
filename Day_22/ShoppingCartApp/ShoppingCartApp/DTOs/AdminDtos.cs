// DTOs/Admin/AdminDtos.cs
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.DTOs.Admin
{
    // ── Dashboard ──

    public class OrderActivityDto
    {
        public string Action { get; set; }
        public string TimeAgo { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }

    public class PromotionDto
    {
        public string Name { get; set; }
        public string Discount { get; set; }
        public int Redemptions { get; set; }
        public string Status { get; set; }
    }

    public class AdminDashboardDto
    {
        // Core metrics
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalReviews { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal MonthlyProfitLoss { get; set; }
        public int LowStockCount { get; set; }
        
        // Percentages
        public double IncomeGrowth { get; set; }
        public double ExpenseGrowth { get; set; }
        public double OrdersGrowth { get; set; }
        public double UsersGrowth { get; set; }
        public double StockTurnover { get; set; }
        public double StockTurnoverGrowth { get; set; }

        // Data arrays for Charts
        public List<string> Months { get; set; }
        public List<decimal> RevenueTrend { get; set; }
        public List<decimal> CostTrend { get; set; }
        public List<decimal> ProfitTrend { get; set; }

        public List<string> CategoryNames { get; set; }
        public List<int> CategorySales { get; set; }
        public List<decimal> CategoryRevenue { get; set; }

        public List<string> PaymentMethods { get; set; }
        public List<int> PaymentCounts { get; set; }

        public List<string> Regions { get; set; }
        public List<int> RegionSales { get; set; }

        // Funnel
        public int FunnelVisitors { get; set; }
        public int FunnelCart { get; set; }
        public int FunnelCheckout { get; set; }
        public int FunnelCompleted { get; set; }

        // Secondary metrics
        public decimal AverageOrderValue { get; set; }
        public int SatisfactionScore { get; set; }
        public int RepeatCustomerRate { get; set; }
        public int OutstandingPayments { get; set; }
        
        // User Registration Trend for Chart
        public List<string> UserRegistrationLabels { get; set; } = new();
        public List<int> UserRegistrationCounts { get; set; } = new();
        
        // Average Order Value Trend for Chart
        public List<string> AOVLabels { get; set; } = new();
        public List<decimal> AOVValues { get; set; } = new();

        // Collections
        public IList<Models.Order> RecentOrders { get; set; }
        public IList<Models.Product> LowStockProducts { get; set; }
        public IList<Models.Product> TopSellingProducts { get; set; }
        public IList<OrderActivityDto> OrderTimeline { get; set; }
        public IList<PromotionDto> ActivePromotions { get; set; }
    }

    // ── Products ──

    public class CreateProductDto
    {
        public ProductViewModel Model { get; set; }
        public string WebRootPath { get; set; }
    }

    public class EditProductDto
    {
        public int Id { get; set; }
        public ProductViewModel Model { get; set; }
        public string WebRootPath { get; set; }
    }

    // ── Orders ──

    public class AdminOrdersDto
    {
        public IList<Models.Order> Orders { get; set; }
        public Dictionary<string, string> UserEmails { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        
        // Order Status Distribution for Chart
        public List<string> StatusLabels { get; set; } = new();
        public List<int> StatusCounts { get; set; } = new();
        
        // Average Order Value Trend
        public List<string> AOVLabels { get; set; } = new();
        public List<decimal> AOVValues { get; set; } = new();
    }

    public class AdminOrderDetailDto
    {
        public Models.Order Order { get; set; }
        public string UserEmail { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
    }

    // ── Reviews Result ──

    public class AdminReviewsResultDto
    {
        public IList<Models.Review> Reviews { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        
        // Rating Distribution for Chart
        public List<int> RatingDistribution { get; set; } = new(); // [1-star count, 2-star count, etc.]
    }

    // ── Import / Export ──

    public class ImportFileDto
    {
        public IFormFile File { get; set; }
    }
}
