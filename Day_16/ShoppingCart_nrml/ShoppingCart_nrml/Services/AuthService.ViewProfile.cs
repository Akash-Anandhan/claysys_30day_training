using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public partial class AuthService
    {
        public async Task<UserProfileDto> ViewProfileAsync()
        {
            var userId = _userContextService.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var roles = await _userManager.GetRolesAsync(user.Id);

            return new UserProfileDto
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = roles.FirstOrDefault()
            };
        }
    }
}


