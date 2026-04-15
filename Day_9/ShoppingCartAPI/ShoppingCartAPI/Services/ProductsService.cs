using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService : IProductsService
    {
        private readonly ShopDbContext _context;

        public ProductsService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            });
        }

        public async Task<ProductDto> GetProductAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name
            };
        }

        public async Task<IEnumerable<ProductDto>> GetYouMayLikeAsync(string userId)
        {
            var cartCategoryIds = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .Select(c => c.Product.CategoryId)
                .Distinct()
                .ToListAsync();

            if (!cartCategoryIds.Any())
            {
                return new List<ProductDto>();
            }

            var cartProductIds = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Select(c => c.ProductId)
                .ToListAsync();

            var recommendedProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => cartCategoryIds.Contains(p.CategoryId) && !cartProductIds.Contains(p.Id))
                .Take(5)
                .ToListAsync();

            return recommendedProducts.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            });
        }

        public async Task<ProductDto> PostProductAsync(ProductDto productDto)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == productDto.CategoryId);

            if (!categoryExists)
            {
                throw new Exception("Invalid CategoryId. Category does not exist.");
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                ImageUrl = productDto.ImageUrl,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto.Id = product.Id;

            return productDto;
        }

        public async Task<string> PutProductAsync(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
                throw new ArgumentException("Product ID mismatch");

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.ImageUrl = productDto.ImageUrl;
            product.Stock = productDto.Stock;
            product.CategoryId = productDto.CategoryId;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                    throw new KeyNotFoundException("Product not found");
                else
                    throw;
            }

            return "Product updated successfully";
        }

        public async Task<string> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return "Product deleted successfully";
        }

        public async Task<object> AddProductsBulkAsync(List<ProductDto> products)
        {

            var productEntities = new List<Product>();

            foreach (var dto in products)
            {
                var product = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    ImageUrl = dto.ImageUrl,
                    Stock = dto.Stock,
                    CategoryId = dto.CategoryId
                };

                productEntities.Add(product);
            }

            await _context.Products.AddRangeAsync(productEntities);
            await _context.SaveChangesAsync();

            return new
            {
                Message = "Products added successfully",
                Count = productEntities.Count
            };
        }
    }
}
