using Application.DTOs.Product;
using Application.Interfaces.Product;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller
{
    public class ProductTagController : BaseController
    {
        private readonly IProductTagService _productTagService;

        public ProductTagController(IProductTagService productTagService)
        {
            _productTagService = productTagService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductTagDto>>> GetAll()
        {
            var tags = await _productTagService.GetAllAsync();
            return Ok(tags);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductTagDto>> GetById(int id)
        {
            var tag = await _productTagService.GetByIdAsync(id);

            if (tag is null)
                throw new NotFoundException("Tag not found.");

            return Ok(tag);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductTagDto>> Create(CreateProductTagDto dto)
        {
            var created = await _productTagService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = created.ProductTagId }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateProductTagDto dto)
        {
            var updated = await _productTagService.UpdateAsync(id, dto);

            if (!updated)
                throw new NotFoundException("Tag not found.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productTagService.DeleteAsync(id);

            if (!deleted)
                throw new NotFoundException("Tag not found.");

            return NoContent();
        }
    }
}