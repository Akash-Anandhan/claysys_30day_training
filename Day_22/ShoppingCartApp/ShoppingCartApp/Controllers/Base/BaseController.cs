// Controllers/Base/BaseController.cs
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Services;
using System.Security.Claims;

namespace ShoppingCartApp.Controllers.Base
{
    public class BaseController : Controller
    {
        // Resolves user ID from claims, or creates/retrieves a guest session ID
        protected string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
                return userId;

            var guestId = HttpContext.Session.GetString(GuestSessionKey);
            if (string.IsNullOrEmpty(guestId))
            {
                guestId = GuestIdPrefix + Guid.NewGuid();
                HttpContext.Session.SetString(GuestSessionKey, guestId);
            }
            return guestId;
        }

        // Returns authenticated user ID only (no guest fallback)
        protected string GetAuthenticatedUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // Handles ServiceResponse pattern for thin controllers
        protected IActionResult ExecuteServiceResponse(ServiceResponse response)
        {
            foreach (var (key, value) in response.TempData)
                TempData[key] = value;

            foreach (var (key, message) in response.ModelErrors)
                ModelState.AddModelError(key, message);

            if (response.Succeeded && response.RedirectAction != null)
                return response.RedirectController != null
                    ? RedirectToAction(response.RedirectAction, response.RedirectController, response.RouteValues)
                    : RedirectToAction(response.RedirectAction, response.RouteValues);

            return View(response.ViewName, response.ViewModel);
        }

        // Common constants for session
        public const string GuestSessionKey = "GuestId";
        public const string GuestIdPrefix = "guest_";
    }
}