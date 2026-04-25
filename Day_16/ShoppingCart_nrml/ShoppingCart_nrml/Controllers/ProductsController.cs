using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Services;

namespace ShoppingCartAPI.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly ShoppingCartAPI.Services.IProductsService _productsService;

        public ProductsController(ShoppingCartAPI.Services.IProductsService productsService) { _productsService = productsService; }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetProducts()
        {
            var dtos = await _productsService.GetProductsAsync();
            return Ok(dtos);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            var dto = await _productsService.GetProductAsync(id);
            return Ok(dto);
        }

        [HttpGet]
        [Route("{id:int}/details")]
        public async Task<IHttpActionResult> GetProductDetails(int id)
        {
            var dto = await _productsService.GetProductDetailsAsync(id);
            return Ok(dto);
        }

        [HttpGet]
        [Route("Suggestions")]
        [Authorize]
        public async Task<IHttpActionResult> GetYouMayLike()
        {
            var dtos = await _productsService.GetYouMayLikeAsync();
            return Ok(dtos);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> PostProduct(ProductDto productDto)
        {
            var createdProduct = await _productsService.PostProductAsync(productDto);
            return Ok(createdProduct);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> PutProduct(int id, ProductDto productDto)
        {
            var resultMessage = await _productsService.PutProductAsync(id, productDto);
            return Ok(new { message = resultMessage });
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            var resultMessage = await _productsService.DeleteProductAsync(id);
            return Ok(new { message = resultMessage });
        }

        [HttpPost]
        [Route("bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> AddProductsBulk(List<ProductDto> products)
        {
            var result = await _productsService.AddProductsBulkAsync(products);
            return Ok(result);
        }

        [HttpPost]
        [Route("{id:int}/reviews")]
        [Authorize]
        public async Task<IHttpActionResult> PostReview(int id, CreateReviewDto dto)
        {
            var review = await _productsService.AddReviewAsync(id, dto);
            return Ok(review);
        }

        [HttpPost]
        [Route("{id:int}/offers")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> PostOffer(int id, CreateOfferDto dto)
        {
            var offer = await _productsService.AddOfferAsync(id, dto);
            return Ok(offer);
        }
    }
}



