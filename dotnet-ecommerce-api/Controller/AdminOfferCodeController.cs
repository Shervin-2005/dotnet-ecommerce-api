using Application.DTOs.OfferCode;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller;

[ApiController]
[Route("api/admin/offer-codes")]
[Authorize(Roles = "Admin")]
public class AdminOfferCodeController : ControllerBase
{
    private readonly IOfferCodeService _offerCodeService;

    public AdminOfferCodeController(IOfferCodeService offerCodeService)
    {
        _offerCodeService = offerCodeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<OfferCodeDto>>> GetAll()
    {
        var offerCodes = await _offerCodeService.GetAllAsync();

        return Ok(offerCodes);
    }

    [HttpGet("{offerCodeId:int}")]
    public async Task<ActionResult<OfferCodeDto>> GetById(int offerCodeId)
    {
        var offerCode = await _offerCodeService.GetByIdAsync(offerCodeId);

        return Ok(offerCode);
    }

    [HttpPost]
    public async Task<ActionResult<OfferCodeDto>> Create(CreateOfferCodeDto dto)
    {
        var offerCode = await _offerCodeService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetById),
            new { offerCodeId = offerCode.OfferCodeId },
            offerCode);
    }

    [HttpPut("{offerCodeId:int}")]
    public async Task<ActionResult<OfferCodeDto>> Update(int offerCodeId, UpdateOfferCodeDto dto)
    {
        var offerCode =
            await _offerCodeService.UpdateAsync(offerCodeId, dto);

        return Ok(offerCode);
    }

    [HttpDelete("{offerCodeId:int}")]
    public async Task<IActionResult> Delete(int offerCodeId)
    {
        await _offerCodeService.DeleteAsync(offerCodeId);

        return NoContent();
    }
    
    [HttpPatch("{offerCodeId:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int offerCodeId)
    {
        await _offerCodeService.DeactivateAsync(offerCodeId);

        return NoContent();
    }
}