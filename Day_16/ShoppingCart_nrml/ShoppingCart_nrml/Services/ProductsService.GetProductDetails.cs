using System.Linq;
using ShoppingCartAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService
    {
        public async Task<ProductDetailsDto> GetProductDetailsAsync(int id)
        {
            
            var productTask = GetProductAsync(id);
            var reviewTask = _reviewService.GetReviewsByProductIdAsync(id);
            var stockTask = _inventoryService.GetStockAsync(id);
            var offerTask = _offerService.GetOffersByProductIdAsync(id);

            try
            {
                await Task.WhenAll(productTask, reviewTask, stockTask, offerTask);
            }
            catch (Exception ex)
            {
            }

            if (productTask.IsFaulted)
            {
                throw new KeyNotFoundException("Product not found or an error occurred.");
            }

            var product = productTask.Result;
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            var reviews = reviewTask.Status == TaskStatus.RanToCompletion ? reviewTask.Result : new List<ReviewDto>();
            if (reviewTask.IsFaulted)
            {
            }

            var stock = stockTask.Status == TaskStatus.RanToCompletion ? stockTask.Result : 0;
            if (stockTask.IsFaulted)
            {
            }

            var offers = offerTask.Status == TaskStatus.RanToCompletion ? offerTask.Result : new List<OfferDto>();
            if (offerTask.IsFaulted)
            {
            }


            return new ProductDetailsDto
            {
                Product = product,
                Reviews = reviews,
                Offers = offers,
                Stock = stock
            };
        }
    }
}



