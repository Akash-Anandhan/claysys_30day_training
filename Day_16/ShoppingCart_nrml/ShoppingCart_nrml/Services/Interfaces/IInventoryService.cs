using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace ShoppingCartAPI.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<int> GetStockAsync(int productId);
    }
}

