using ShoppingCartAPI.DTOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService
    {
        public async Task<ProductDetailsDto> GetProductDetailsAsync(int id)
        {
            _logger.LogInformation("Starting fetching product details for ProductId {ProductId}", id);
            
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
                _logger.LogError(ex, "One or more parallel tasks failed for ProductId {ProductId}", id);
            }

            if (productTask.IsFaulted)
            {
                _logger.LogError(productTask.Exception, "Failed to fetch product for ProductId: {ProductId}", id);
                throw new KeyNotFoundException("Product not found or an error occurred.");
            }

            var product = productTask.Result;
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            var reviews = reviewTask.IsCompletedSuccessfully ? reviewTask.Result : new List<ReviewDto>();
            if (reviewTask.IsFaulted)
            {
                _logger.LogError(reviewTask.Exception, "Failed to fetch reviews for ProductId: {ProductId}", id);
            }

            var stock = stockTask.IsCompletedSuccessfully ? stockTask.Result : 0;
            if (stockTask.IsFaulted)
            {
                _logger.LogError(stockTask.Exception, "Failed to fetch stock for ProductId: {ProductId}", id);
            }

            var offers = offerTask.IsCompletedSuccessfully ? offerTask.Result : new List<OfferDto>();
            if (offerTask.IsFaulted)
            {
                _logger.LogError(offerTask.Exception, "Failed to fetch offers for ProductId: {ProductId}", id);
            }

            _logger.LogInformation("Completed fetching product details in parallel for ProductId {ProductId}", id);

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
