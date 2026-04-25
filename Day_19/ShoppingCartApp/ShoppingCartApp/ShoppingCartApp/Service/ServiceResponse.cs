// Services/ServiceResponse.cs
namespace ShoppingCartApp.Services
{
    public class ServiceResponse
    {
        public bool Succeeded { get; set; }
        public string RedirectAction { get; set; }
        public string RedirectController { get; set; }
        public object RouteValues { get; set; }
        public string ViewName { get; set; }
        public object ViewModel { get; set; }
        public string SessionRemoveKey { get; set; }

        public Dictionary<string, string> ModelErrors { get; set; } = new();
        public Dictionary<string, string> TempData { get; set; } = new();

        // ── Factories ──────────────────────────────────────────────────────
        public static ServiceResponse Redirect(
            string action,
            string controller = null,
            object routeValues = null) => new()
            {
                Succeeded = true,
                RedirectAction = action,
                RedirectController = controller,
                RouteValues = routeValues
            };

        public static ServiceResponse Redirect(
            string action,
            string controller,
            Dictionary<string, string> tempData) => new()
            {
                Succeeded = true,
                RedirectAction = action,
                RedirectController = controller,
                TempData = tempData
            };

        public static ServiceResponse ShowView(
            string viewName,
            object model = null) => new()
            {
                Succeeded = false,
                ViewName = viewName,
                ViewModel = model
            };

        public static ServiceResponse ShowView(
            string viewName,
            object model,
            Dictionary<string, string> modelErrors) => new()
            {
                Succeeded = false,
                ViewName = viewName,
                ViewModel = model,
                ModelErrors = modelErrors
            };

        public static ServiceResponse ShowView(
            string viewName,
            object model,
            string errorKey,
            string errorMessage) => new()
            {
                Succeeded = false,
                ViewName = viewName,
                ViewModel = model,
                ModelErrors = new Dictionary<string, string> { { errorKey, errorMessage } }
            };
    }
}