using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService
    {
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

