namespace ShoppingCartAPI.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<int> GetStockAsync(int productId);
    }
}
