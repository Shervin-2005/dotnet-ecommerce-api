using Application.DTOs.Brand;
using Application.Interfaces.Brand;
using Domain.Exceptions;
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
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll()
        {
            var brands = await _brandService.GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BrandDto>> GetById(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            
            if (brand is null) 
                throw new NotFoundException(("Brand not found"));
            
            return Ok(brand);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BrandDto>> Create([FromForm] CreateBrandRequest request)
        {
            await using var stream = request.File!.OpenReadStream();

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
        public async Task<IActionResult> Update(int id, [FromForm] UpdateBrandRequest request)
        {
            Stream? imageStream = null;
            
            try
            {
                var dto = new UpdateBrandDto
                {
                    BrandName = request.BrandName
                };

                if (request.File is not null)
                {
                    imageStream = request.File.OpenReadStream();

                    dto.Image = imageStream;
                    dto.ImageName = request.File.FileName;
                    dto.ContentType = request.File.ContentType;
                }

                var updated = await _brandService.UpdateAsync(id, dto);

                if (!updated)
                    throw new NotFoundException("Brand not found.");

                return NoContent();
            }
            finally
            {
                if (imageStream is not null)
                    await imageStream.DisposeAsync();
            }
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _brandService.DeleteAsync(id);
            
            if(!deleted)throw new NotFoundException("Brand not found.");
            
            return NoContent();
        }
    }
}
