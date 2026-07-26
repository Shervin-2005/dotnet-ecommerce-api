using Application.DTOs;
using Application.Interfaces;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;


        public CategoryController(ICategoryService categoryService, IImageStorageService imageStorageService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create([FromForm]CategoryRequest request)
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

            var dto = new CreateCategoryDto
            {
                CategoryName = request.CategoryName,
                Image = stream,
                ImageName = request.File.FileName,
                ContentType = request.File.ContentType
            };

            var created = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.CategoryId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CategoryRequest request)
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

            var dto = new UpdateCategoryDto
            {
                CategoryName = request.CategoryName,
                Image = stream,
                ImageName = request.File.FileName,
                ContentType = request.File.ContentType
            };

            var updated = await _categoryService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
