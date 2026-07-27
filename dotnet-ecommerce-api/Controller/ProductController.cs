using Application.DTOs;
using Application.Interfaces;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromForm]ProductRequest request)
        {
            if (request.Images.Count == 0)
                return BadRequest("At least one image is required");

            const long maxSizeBytes = 5 * 1024 * 1024; //5MB

            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            var dto = new CreateProductDto
            {
                ProductName = request.ProductName,
                Price = request.Price,
                Description = request.Description,
                StockQuantity = request.StockQuantity,
                CategoryId = request.CategoryId,
                BrandId = request.BrandId
            };

            foreach (var file in request.Images)
            {
                if (file.Length == 0)
                    return BadRequest("One of the images is empty.");

                if (file.Length > maxSizeBytes)
                    return BadRequest($"'{file.FileName}' exceeds 5 MB.");

                if (!allowedTypes.Contains(file.ContentType))
                    return BadRequest($"'{file.FileName}' has an unsupported format.");

                dto.Images.Add(new ProductImageDto
                {
                    Image = file.OpenReadStream(),
                    ImageName = file.FileName,
                    ContentType = file.ContentType
                });
            }
            var created = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var updated = await _productService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:int}/images")]
        public async Task<ActionResult<ProductImageDto>> AddImage(int id, IFormFile file, [FromForm] bool isMain = false, [FromForm] int displayOrder = 0)
        {
            //later should alter this with fluent validation 
            if (file is null || file.Length == 0)
                return BadRequest("No file uploaded.");

            const long maxSizeBytes = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxSizeBytes)
                return BadRequest("File too large. Max size is 5 MB.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("Unsupported file type. Use JPEG, PNG, or WebP.");

            await using var stream = file.OpenReadStream();

            var upload = new ProductImageDto
            {
                Image = stream,
                ImageName = file.FileName,
                ContentType = file.ContentType,
                IsMain = isMain,
                DisplayOrder = displayOrder
            };

            var result = await _productService.AddImageAsync(id, upload);
            if (result is false) return NotFound();

            return NoContent();
        }

        [HttpDelete("{productId:int}/images/{imageId:int}")]
        public async Task<IActionResult> RemoveImage(int productId, int imageId)
        {
            var removed = await _productService.RemoveImageAsync(productId, imageId);
            if (!removed) return NotFound();
            return NoContent();
        }

        [HttpPut("{productId:int}/images/{imageId:int}/main")]
        public async Task<IActionResult> SetMainImage(int productId, int imageId)
        {
            var updated = await _productService.SetMainImageAsync(productId, imageId);
            if (!updated) return NotFound();
            return NoContent();
        }
    }
}
