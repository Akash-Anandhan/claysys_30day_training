// Services/AuthService.GetProfileAsync.cs
using Microsoft.AspNetCore.Identity;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class AuthService
    {
        public async Task<ServiceResponse> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResponse.ShowView("NotFound", null, string.Empty, "User not found.");
            
            // Parse stored address into separate fields
            var addressParts = ParseAddressParts(user.Address);
            
            return ServiceResponse.ShowView("Profile", new ShoppingCartApp.ViewModels.ProfileViewModel 
            { 
                FullName = user.FullName, 
                Email = user.Email, 
                PhoneNumber = user.PhoneNumber,
                StreetAddress = addressParts.StreetAddress,
                City = addressParts.City,
                State = addressParts.State,
                PostalCode = addressParts.PostalCode,
                Country = addressParts.Country
            });
        }
        
        // Helper method to parse composite address string into parts
        private (string StreetAddress, string City, string State, string PostalCode, string Country) ParseAddressParts(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return (string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            
            var parts = address.Split(',').Select(p => p.Trim()).ToArray();
            
            string streetAddress = parts.Length > 0 ? parts[0] : string.Empty;
            string city = parts.Length > 1 ? parts[1] : string.Empty;
            string stateAndPostal = parts.Length > 2 ? parts[2] : string.Empty;
            string country = parts.Length > 3 ? parts[3] : string.Empty;
            
            // Try to split state and postal code (e.g., "NY 10001")
            string state = string.Empty;
            string postalCode = string.Empty;
            
            if (!string.IsNullOrWhiteSpace(stateAndPostal))
            {
                var statePostalParts = stateAndPostal.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (statePostalParts.Length > 0) state = statePostalParts[0];
                if (statePostalParts.Length > 1) postalCode = statePostalParts[1];
            }
            
            return (streetAddress, city, state, postalCode, country);
        }
    }
}