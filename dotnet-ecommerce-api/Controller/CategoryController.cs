using Application.DTOs.Category;
using Application.Interfaces;
using Application.Interfaces.Category;
using Domain.Exceptions;
using dotnet_ecommerce_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;


        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null) throw new NotFoundException("Category not found.");
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> Create([FromForm] CreateCategoryRequest request)
        {
            await using var stream = request.File!.OpenReadStream();

            var dto = new CreateCategoryDto
            {
                CategoryName = request.CategoryName,
                Image = stream,
                ImageName = request.File.FileName,
                ContentType = request.File.ContentType
            };

            var created = await _categoryService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.CategoryId },
                created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateCategoryRequest request)
        {
            Stream? imageStream = null;

            try
            {
                var dto = new UpdateCategoryDto
                {
                    CategoryName = request.CategoryName
                };

                if (request.File is not null)
                {
                    imageStream = request.File.OpenReadStream();

                    dto.Image = imageStream;
                    dto.ImageName = request.File.FileName;
                    dto.ContentType = request.File.ContentType;
                }

                var updated = await _categoryService.UpdateAsync(id, dto);

                if (!updated)
                    throw new NotFoundException("Category not found.");

                return NoContent();
            }
            finally
            {
                if (imageStream is not null) await imageStream.DisposeAsync();
            }
        }
        
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);
            if (!deleted) throw new NotFoundException("Category not found.");
            return NoContent();
        }
    }
}
