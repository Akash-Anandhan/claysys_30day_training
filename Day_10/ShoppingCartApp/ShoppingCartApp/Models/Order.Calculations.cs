namespace ShoppingCartApp.Models
{
    public partial class Order
    {
      
        public int TotalItemCount
        {
            get
            {
                if (OrderItems == null || !OrderItems.Any())
                    return 0;

                return OrderItems.Sum(item => item.Quantity);
            }
        }

        public bool IsPlacedToday =>
            OrderDate.Date == DateTime.Today;
        public string OrderAgeLabel
        {
            get
            {
                var diff = DateTime.Now - OrderDate;

                if (diff.TotalMinutes < 1)
                    return "Just now";
                if (diff.TotalMinutes < 60)
                    return $"{(int)diff.TotalMinutes} minutes ago";
                if (diff.TotalHours < 24)
                    return $"{(int)diff.TotalHours} hours ago";
                if (diff.TotalDays < 7)
                    return $"{(int)diff.TotalDays} days ago";

                return OrderDate.ToString("MMM dd, yyyy");
            }
        }

        public bool IsPending => Status == "Pending";
        public bool IsShipped => Status == "Shipped";
        public bool IsDelivered => Status == "Delivered";
        public bool IsCancelled => Status == "Cancelled";

        public string StatusBadgeColor => Status switch
        {
            "Pending" => "warning",
            "Shipped" => "primary",
            "Delivered" => "success",
            "Cancelled" => "danger",
            _ => "secondary"
        };

        public string GetOrderSummary()
        {
            var itemCount = TotalItemCount;
            var itemWord = itemCount == 1 ? "item" : "items";

            return $"Order #{Id} — {itemCount} {itemWord} " +
                   $"totalling ${TotalAmount:0.00} " +
                   $"placed {OrderAgeLabel}. " +
                   $"Status: {Status}.";
        }
    }
}