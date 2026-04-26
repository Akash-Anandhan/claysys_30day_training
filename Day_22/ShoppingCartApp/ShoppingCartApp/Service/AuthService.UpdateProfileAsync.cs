// Services/AuthService.UpdateProfileAsync.cs
using Microsoft.AspNetCore.Identity;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Services
{
    public partial class AuthService
    {
        public async Task<ServiceResponse> UpdateProfileAsync(UpdateProfileDto dto, ProfileViewModel model = null)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return ServiceResponse.ShowView("Profile", null, string.Empty, "User not found.");
            user.FullName = dto.FullName;
            user.Address = dto.Address;
            
            // Update phone number if provided via model
            if (model?.PhoneNumber != null)
            {
                user.PhoneNumber = model.PhoneNumber;
            }
            
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return ServiceResponse.ShowView("Profile", new UpdateProfileDto { FullName = dto.FullName, Address = dto.Address }, result.Errors.ToDictionary(_ => string.Empty, e => e.Description));
            return ServiceResponse.Redirect("Profile", "Account", new Dictionary<string, string> { { "Success", "Profile updated successfully!" } });
        }
    }
}