using Application.DTOs.Product;
using Application.Interfaces.Product;
using Domain.Exceptions;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
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
            if (product is null) throw new NotFoundException("Product not found.");
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> Create([FromForm] ProductRequest request)
        {
            var streams = new List<Stream>();

            try
            {
                var dto = new CreateProductDto
                {
                    ProductName = request.ProductName,
                    OriginalPrice = request.OriginalPrice,
                    SalePrice = request.SalePrice,
                    SaleDurationMinutes = request.SaleDurationMinutes,
                    Description = request.Description,
                    StockQuantity = request.StockQuantity,
                    CategoryId = request.CategoryId,
                    BrandId = request.BrandId
                };

                foreach (var file in request.Images)
                {
                    var stream = file.OpenReadStream();
                    streams.Add(stream);

                    dto.Images.Add(new ProductImageUploadDto
                    {
                        Image = stream,
                        ImageName = file.FileName,
                        ContentType = file.ContentType
                    });
                }

                var created = await _productService.CreateAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
            }
            finally
            {
                foreach (var stream in streams)
                {
                    await stream.DisposeAsync();
                }
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var updated = await _productService.UpdateAsync(id, dto);
            if (!updated) throw new NotFoundException("Product not found.");
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted) throw new NotFoundException("Product not found.");
            return NoContent();
        }

        [HttpPost("{id:int}/images")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddImage(int id, [FromForm] AddProductImageRequest request)
        {
            await using var stream = request.File!.OpenReadStream();

            var upload = new ProductImageUploadDto
            {
                Image = stream,
                ImageName = request.File.FileName,
                ContentType = request.File.ContentType,
                IsMain = request.IsMain,
                DisplayOrder = request.DisplayOrder
            };

            var result = await _productService.AddImageAsync(id, upload);

            if (!result)
                throw new NotFoundException("Product not found.");

            return NoContent();
        }

        [HttpDelete("{productId:int}/images/{imageId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveImage(int productId, int imageId)
        {
            var removed = await _productService.RemoveImageAsync(productId, imageId);
            if (!removed) return NotFound();
            return NoContent();
        }

        [HttpPut("{productId:int}/images/{imageId:int}/main")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetMainImage(int productId, int imageId)
        {
            var updated = await _productService.SetMainImageAsync(productId, imageId);
            if (!updated) return NotFound();
            return NoContent();
        }
        [HttpPut("{id:int}/specifications")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetSpecifications(int id, List<UpsertProductSpecificationDto> specs)
        {
            var updated = await _productService.SetSpecificationsAsync(id, specs);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpPut("{id:int}/attributes")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetAttributes(int id, SetProductAttributesDto dto)
        {
            var updated = await _productService.SetAttributesAsync(id, dto);

            if (!updated) return NotFound();

            return NoContent();
        }
    }
}
