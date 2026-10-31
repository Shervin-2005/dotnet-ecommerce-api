using Application.DTOs.Brand;
using Application.Interfaces.Brand;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller;

public class BrandGroupController : BaseController
{
    private readonly IBrandGroupService _brandGroupService;

    public BrandGroupController(IBrandGroupService brandGroupService)
    {
        _brandGroupService = brandGroupService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BrandGroupDto>>> GetAll()
    {
        var groups = await _brandGroupService.GetAllWithBrandsAsync();
        return Ok(groups);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BrandGroupDto>> GetById(int id)
    {
        var group = await _brandGroupService.GetByIdAsync(id);

        if (group is null)
            throw new NotFoundException("Brand group not found.");

        return Ok(group);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandGroupDto>> Create([FromBody] CreateBrandGroupDto dto)
    {
        var group = await _brandGroupService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = group.BrandGroupId }, group);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandGroupDto dto)
    {
        var updated = await _brandGroupService.UpdateAsync(id, dto);

        if (!updated)
            throw new NotFoundException("Brand group not found.");

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _brandGroupService.DeleteAsync(id);

        if (!deleted)
            throw new NotFoundException("Brand group not found.");

        return NoContent();
    }
}