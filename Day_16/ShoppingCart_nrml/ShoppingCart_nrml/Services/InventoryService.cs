using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ShoppingCartAPI.Services.Interfaces;

namespace ShoppingCartAPI.Services
{
    public class InventoryService : IInventoryService
    {
        public async Task<int> GetStockAsync(int productId)
        {
            // Simulate network delay
            await Task.Delay(100);

            // Dummy implementation: returns random stock
            var random = new Random();
            return random.Next(0, 100);
        }
    }
}

