using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Services;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var dtos = await _productsService.GetProductsAsync();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var dto = await _productsService.GetProductAsync(id);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("Suggestions")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetYouMayLike()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dtos = await _productsService.GetYouMayLikeAsync(userId);
            return Ok(dtos);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto productDto)
        {
            try
            {
                var createdProduct = await _productsService.PostProductAsync(productDto);

                return CreatedAtAction(
                    nameof(GetProduct),
                    new { id = createdProduct.Id },
                    createdProduct
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred."
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProduct(int id, ProductDto productDto)
        {
            try
            {
                var resultMessage = await _productsService.PutProductAsync(id, productDto);
                return Ok(new { message = resultMessage });
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var resultMessage = await _productsService.DeleteProductAsync(id);
                return Ok(new { message = resultMessage });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProductsBulk([FromBody] List<ProductDto> products)
        {
            try
            {
                var result = await _productsService.AddProductsBulkAsync(products);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}