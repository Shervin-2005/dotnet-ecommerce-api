using Application.DTOs;
using Application.Interfaces;
using Azure.Core;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller
{
    public class BrandController : BaseController
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll()
        {
            var brands = await _brandService.GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BrandDto>> GetById(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand is null) return NotFound();
            return Ok(brand);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BrandDto>> Create([FromForm]BrandRequest request)
        {
            //later should alter this with fluent validation 
            if (request.File is null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            const long maxSizeBytes = 5 * 1024 * 1024; // 5 MB
            if (request.File.Length > maxSizeBytes)
                return BadRequest("File too large. Max size is 5 MB.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(request.File.ContentType))
                return BadRequest("Unsupported file type. Use JPEG, PNG, or WebP.");

            await using var stream = request.File.OpenReadStream();

            var dto = new CreateBrandDto
            {
                BrandName = request.BrandName,
                Image = stream,
                ImageName = request.File.FileName,
                ContentType = request.File.ContentType
            };

            var created = await _brandService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.BrandId }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, BrandRequest request)
        {
            //later should alter this with fluent validation 
            if (request.File is null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            const long maxSizeBytes = 5 * 1024 * 1024; // 5 MB
            if (request.File.Length > maxSizeBytes)
                return BadRequest("File too large. Max size is 5 MB.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(request.File.ContentType))
                return BadRequest("Unsupported file type. Use JPEG, PNG, or WebP.");

            await using var stream = request.File.OpenReadStream();

            var dto = new UpdateBrandDto
            {
                BrandName = request.BrandName,
                Image = stream,
                ImageName = request.File.FileName,
                ContentType = request.File.ContentType
            };

            var updated = await _brandService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _brandService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
