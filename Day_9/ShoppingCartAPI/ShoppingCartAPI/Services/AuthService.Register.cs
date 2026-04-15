using Microsoft.AspNetCore.Identity;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Services
{
    public partial class AuthService
    {
        public async Task<string> RegisterAsync(RegisterDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                throw new InvalidOperationException("User already exists!");

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                Address = model.Address,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            await _userManager.AddToRoleAsync(user, model.Role);

            return "User created successfully";
        }
    }
}
