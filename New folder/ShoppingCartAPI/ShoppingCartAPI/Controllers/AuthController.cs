// Controllers/AuthController.cs
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Web.Http;

[RoutePrefix("api/auth")]
public class AuthController : ApiController
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController()
    {
        var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        _userManager = new UserManager<ApplicationUser>(store);
    }

    [HttpPost, Route("register")]
    public async Task<IHttpActionResult> Register(RegisterModel model)
    {
        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return BadRequest(string.Join(", ", result.Errors));
        await _userManager.AddToRoleAsync(user.Id, "User"); // default role
        return Ok("Registered successfully");
    }

    [HttpPost, Route("login")]
    public async Task<IHttpActionResult> Login(LoginModel model)
    {
        var user = await _userManager.FindAsync(model.Email, model.Password);
        if (user == null) return Unauthorized();
        var roles = await _userManager.GetRolesAsync(user.Id);
        var token = JwtHelper.GenerateToken(user.Id, user.Email, roles);
        return Ok(new { token, email = user.Email, roles });
    }
}

// DTOs
public class RegisterModel { public string FullName { get; set; } public string Email { get; set; } public string Password { get; set; } }
public class LoginModel { public string Email { get; set; } public string Password { get; set; } }