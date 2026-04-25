using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace ShoppingCartAPI.DTOs
{
    public class TokenApiDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

