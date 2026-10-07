using Application.DTOs.Product;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller;

public class ProductVariantController : BaseController
{
    private readonly IProductVariantService _variantService;

    public ProductVariantController(IProductVariantService variantService)
    {
        _variantService = variantService;
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> Get(int productId)
    {
        return Ok();
    }

    [HttpGet("{productId:int}/{variantId:int}")]
    public async Task<IActionResult> GetById(int productId, int variantId)
    {
        var variant = await _variantService.GetByIdAsync(productId, variantId);

        if (variant is null)
            return NotFound();

        return Ok(variant);
    }

    [HttpPost("{productId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductVariantDto>> Create(int productId, CreateProductVariantDto dto)
    {
        var variant = await _variantService.CreateAsync(productId, dto);

        return Ok(variant);
    }

    [HttpPut("{productId:int}/{variantId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int productId, int variantId, UpdateProductVariantDto dto)
    {
        var updated = await _variantService.UpdateAsync(productId, variantId, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{productId:int}/{variantId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int productId, int variantId)
    {
        var deleted = await _variantService.DeleteAsync(productId, variantId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}