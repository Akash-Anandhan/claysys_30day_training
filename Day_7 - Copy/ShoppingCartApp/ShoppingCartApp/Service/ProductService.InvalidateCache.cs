// Services/ProductService.InvalidateCache.cs
namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        public void InvalidateProductsCache()
        {
            // Note: MemoryCache doesn't support pattern-based invalidation
            // For full invalidation, consider using distributed cache or cache tags
        }

        public void InvalidateProductDetailCache(int productId)
        {
            _cache.Remove(string.Format("product_{0}", productId));
            _cache.Remove(string.Format("recommendations_{0}", productId));
        }
    }
}