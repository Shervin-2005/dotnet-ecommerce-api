using Application.DTOs.FlashSale;
using Application.Interfaces.FlashSale;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller;

[ApiController]
[Authorize(Roles = "Admin")]
public class FlashSaleController : BaseController
{
    private readonly IFlashSaleService _flashSaleService;

    public FlashSaleController(IFlashSaleService flashSaleService)
    {
        _flashSaleService = flashSaleService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFlashSaleDto dto, CancellationToken cancellationToken)
    {
        var result = await _flashSaleService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { flashSaleId = result.FlashSaleId }, result);
    }
    
    [HttpPut("{flashSaleId:int}")]
    public async Task<IActionResult> Update(int flashSaleId, [FromBody] UpdateFlashSaleDto dto,CancellationToken cancellationToken)
    {
        await _flashSaleService.UpdateAsync(flashSaleId, dto, cancellationToken);

        return NoContent();
    }
    
    [HttpGet("{flashSaleId:int}")]
    public async Task<IActionResult> GetById(int flashSaleId, CancellationToken cancellationToken)
    {
        var result = await _flashSaleService.GetByIdAsync(flashSaleId, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
    
    [HttpPost("{flashSaleId:int}/items")]
    public async Task<IActionResult> AddItems(int flashSaleId, [FromBody] AddFlashSaleItemDto dto, CancellationToken cancellationToken)
    {
        await _flashSaleService.AddItemsAsync(flashSaleId, dto, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{flashSaleId:int}/items/{itemId:int}")]
    public async Task<IActionResult> RemoveItem(int flashSaleId, int itemId, CancellationToken cancellationToken)
    {
        await _flashSaleService.RemoveItemAsync(flashSaleId, itemId, cancellationToken);

        return NoContent();
    }
}