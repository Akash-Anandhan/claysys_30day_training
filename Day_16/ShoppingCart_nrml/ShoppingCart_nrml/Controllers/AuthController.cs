using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Services;

namespace ShoppingCartAPI.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly ShoppingCartAPI.Services.IAuthService _authService;

        public AuthController(ShoppingCartAPI.Services.IAuthService authService) { _authService = authService; }

        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register(RegisterDto model)
        {
            var result = await _authService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginDto model)
        {
            var result = await _authService.LoginAsync(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IHttpActionResult> Refresh(TokenApiDto tokenApiDto)
        {
            var result = await _authService.RefreshAsync(tokenApiDto);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        [Route("view")]
        public async Task<IHttpActionResult> ViewProfile()
        {
            var result = await _authService.ViewProfileAsync();
            return Ok(result);
        }
    }
}



