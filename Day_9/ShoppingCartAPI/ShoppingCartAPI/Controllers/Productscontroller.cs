using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Services;
using System.Security.Claims;
using ShoppingCartAPI.Services.Interfaces;

namespace ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductsService productsService, 
            ILogger<ProductsController> logger)
        {
            _productsService = productsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            _logger.LogInformation("Fetching all products.");
            var dtos = await _productsService.GetProductsAsync();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var dto = await _productsService.GetProductAsync(id);
            return Ok(dto);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<ProductDetailsDto>> GetProductDetails(int id)
        {
            var dto = await _productsService.GetProductDetailsAsync(id);
            return Ok(dto);
        }

        [HttpGet("Suggestions")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetYouMayLike()
        {
            var dtos = await _productsService.GetYouMayLikeAsync();
            return Ok(dtos);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto productDto)
        {
            var createdProduct = await _productsService.PostProductAsync(productDto);

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = createdProduct.Id },
                createdProduct
            );
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProduct(int id, ProductDto productDto)
        {
            var resultMessage = await _productsService.PutProductAsync(id, productDto);
            return Ok(new { message = resultMessage });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var resultMessage = await _productsService.DeleteProductAsync(id);
            return Ok(new { message = resultMessage });
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProductsBulk([FromBody] List<ProductDto> products)
        {
            var result = await _productsService.AddProductsBulkAsync(products);
            return Ok(result);
        }

        [HttpPost("import/excel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportProductsFromExcel(Microsoft.AspNetCore.Http.IFormFile file)
        {
            var count = await _productsService.ImportProductsFromExcelAsync(file);
            return Ok(new { message = $"Successfully imported {count} products from Excel." });
        }

        [HttpPost("import/csv")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportProductsFromCsv(Microsoft.AspNetCore.Http.IFormFile file)
        {
            var count = await _productsService.ImportProductsFromCsvAsync(file);
            return Ok(new { message = $"Successfully imported {count} products from CSV." });
        }

        [HttpGet("export/excel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportProductsToExcel()
        {
            var fileBytes = await _productsService.ExportProductsToExcelAsync();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
        }

        [HttpGet("export/csv")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportProductsToCsv()
        {
            var fileBytes = await _productsService.ExportProductsToCsvAsync();
            return File(fileBytes, "text/csv", "Products.csv");
        }

        [HttpPost("{id}/reviews")]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> PostReview(int id, [FromBody] CreateReviewDto dto)
        {
            var review = await _productsService.AddReviewAsync(id, dto);
            return Ok(review);
        }

        [HttpPost("{id}/offers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OfferDto>> PostOffer(int id, [FromBody] CreateOfferDto dto)
        {
            var offer = await _productsService.AddOfferAsync(id, dto);
            return Ok(offer);
        }
    }
}