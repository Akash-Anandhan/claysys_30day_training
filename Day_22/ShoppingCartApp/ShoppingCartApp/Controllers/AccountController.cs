// Controllers/AccountController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Services;
using ShoppingCartApp.ViewModels;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Controllers.Base;
using System.Security.Claims;

namespace ShoppingCartApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;

        public AccountController(IAuthService authService, IOrderService orderService)
        {
            _authService = authService;
            _orderService = orderService;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var dto = new RegisterDto
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password
            };
            return ExecuteServiceResponse(await _authService.RegisterAsync(dto));
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var dto = new LoginDto
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe,
                GuestId = HttpContext.Session.GetString(BaseController.GuestSessionKey)
            };
            return ExecuteServiceResponse(await _authService.LoginAsync(dto));
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            return ExecuteServiceResponse(await _authService.LogoutAsync());
        }

        // GET: /Account/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = GetAuthenticatedUserId();
            return ExecuteServiceResponse(await _authService.GetProfileAsync(userId));
        }

        // POST: /Account/Profile
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var address = $"{model.StreetAddress}, {model.City}, {model.State} {model.PostalCode}, {model.Country}";
            var dto = new UpdateProfileDto
            {
                UserId = GetAuthenticatedUserId(),
                FullName = model.FullName,
                Address = address
            };
            return ExecuteServiceResponse(await _authService.UpdateProfileAsync(dto, model));
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() => View();

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var resetUrl = Url.Action(
                "ResetPassword", "Account",
                new { token = "__TOKEN__", email = model.Email },
                Request.Scheme);

            var dto = new ForgotPasswordDto
            {
                Email = model.Email,
                ResetUrl = resetUrl
            };
            return ExecuteServiceResponse(await _authService.GeneratePasswordResetTokenAsync(dto));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation() => View();

        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token = null, string email = null)
        {
            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var dto = new ResetPasswordDto
            {
                Email = model.Email,
                Token = model.Token,
                Password = model.Password
            };
            return ExecuteServiceResponse(await _authService.ResetPasswordAsync(dto));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation() => View();

        // GET: /Account/Orders
        public async Task<IActionResult> Orders(string? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1)
        {
            var userId = GetAuthenticatedUserId();
            return ExecuteServiceResponse(await _orderService.GetUserOrdersAsync(userId, status, fromDate, toDate, page));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = GetAuthenticatedUserId();
            return ExecuteServiceResponse(await _orderService.CancelOrderAsync(id, userId));
        }

        // GET: /Account/TrackOrder/5
        [Authorize]
        public async Task<IActionResult> TrackOrder(int id)
        {
            var userId = GetAuthenticatedUserId();
            var order = await _orderService.GetOrderByIdAsync(id, userId);
            
            if (order == null)
            {
                TempData["Error"] = "Order not found or you don't have permission to view it.";
                return RedirectToAction("Orders");
            }
            
            return View(order);
        }
    }
}
