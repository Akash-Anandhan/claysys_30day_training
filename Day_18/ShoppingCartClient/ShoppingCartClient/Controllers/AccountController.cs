using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartClient.Models;
using ShoppingCartClient.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShoppingCartClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiService _apiService;

        public AccountController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return View(loginDto);

            var response = await _apiService.LoginAsync(loginDto);
            
            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                // Store token in Session
                HttpContext.Session.SetString("JWTToken", response.Token);

                // Create Cookie Auth Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, response.FullName ?? response.Email),
                    new Claim(ClaimTypes.Email, response.Email)
                };

                if (!string.IsNullOrEmpty(response.Role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, response.Role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Product");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(loginDto);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return View(registerDto);

            var success = await _apiService.RegisterAsync(registerDto);

            if (success)
            {
                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed. Email might already exist.");
            return View(registerDto);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
