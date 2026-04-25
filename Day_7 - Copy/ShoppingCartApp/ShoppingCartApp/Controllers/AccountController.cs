// Controllers/AccountController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Services;
using ShoppingCartApp.ViewModels;
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
            return Execute(await _authService.RegisterAsync(new RegisterDto
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password
            }));
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
            return Execute(await _authService.LoginAsync(new LoginDto
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe,
                GuestId = HttpContext.Session.GetString("GuestId")
            }));
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            return Execute(await _authService.LogoutAsync());
        }

        // GET: /Account/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            return Execute(await _authService.GetProfileAsync(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value));
        }

        // POST: /Account/Profile
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            return Execute(await _authService.UpdateProfileAsync(new UpdateProfileDto
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                FullName = model.FullName,
                Address = model.Address
            }));
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
            // Url.Action() stays here — it needs IUrlHelper which is
            // an HTTP/controller concern, not a service concern.
            var resetUrl = Url.Action(
                "ResetPassword", "Account",
                new { token = "__TOKEN__", email = model.Email },
                Request.Scheme);

            return Execute(await _authService.GeneratePasswordResetTokenAsync(new ForgotPasswordDto
            {
                Email = model.Email,
                ResetUrl = resetUrl
            }));
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
            return Execute(await _authService.ResetPasswordAsync(new ResetPasswordDto
            {
                Email = model.Email,
                Token = model.Token,
                Password = model.Password
            }));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation() => View();

        // GET: /Account/Orders
        public async Task<IActionResult> Orders()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var response = await _orderService.GetUserOrdersAsync(userId);
            return Execute(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var response = await _orderService.CancelOrderAsync(id, userId);
            return Execute(response);
        }
    }
}
