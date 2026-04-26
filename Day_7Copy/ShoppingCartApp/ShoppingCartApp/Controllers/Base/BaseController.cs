// Controllers/Base/BaseController.cs
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Services;
using System.Security.Claims;

namespace ShoppingCartApp.Controllers.Base
{
    public class BaseController : Controller
    {
        // Resolves the current authenticated user's ID from claims.
        // Available to every controller that inherits BaseController.
        protected string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        protected IActionResult Execute(ServiceResponse response)
        {
            foreach (var (key, value) in response.TempData)
                TempData[key] = value;

            foreach (var (key, message) in response.ModelErrors)
                ModelState.AddModelError(key, message);

            if (response.SessionRemoveKey == "__ALL__")
                HttpContext.Session.Clear();
            else if (response.SessionRemoveKey != null)
                HttpContext.Session.Remove(response.SessionRemoveKey);

            if (response.Succeeded && response.RedirectAction != null)
                return response.RedirectController != null
                    ? RedirectToAction(response.RedirectAction, response.RedirectController, response.RouteValues)
                    : RedirectToAction(response.RedirectAction, response.RouteValues);

            return View(response.ViewName, response.ViewModel);
        }
    }
}