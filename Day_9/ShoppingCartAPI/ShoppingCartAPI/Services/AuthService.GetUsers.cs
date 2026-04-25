using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Services
{
    public partial class AuthService
    {
        public async Task<IEnumerable<UserProfileDto>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userProfileDtos = new List<UserProfileDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userProfileDtos.Add(new UserProfileDto
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }

            return userProfileDtos;
        }
    }
}
