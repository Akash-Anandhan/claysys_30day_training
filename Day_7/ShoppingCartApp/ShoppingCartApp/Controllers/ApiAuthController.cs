// Controllers/ApiAuthController.cs
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Services;

namespace ShoppingCartApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiAuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public ApiAuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        // POST: api/ApiAuth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApiLoginDto model)
        {
            if (!ModelState.IsValid)
                return Unauthorized("Invalid login attempt.");

            var result = await _tokenService.GenerateTokenAsync(model.Email, model.Password);
            if (result == null)
                return Unauthorized("Invalid login attempt.");

            return Ok(result);
        }

        // POST: api/ApiAuth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload.");

            var result = await _tokenService.RefreshTokenAsync(model);
            if (result == null)
                return BadRequest("Invalid token.");

            return Ok(result);
        }
    }
}
