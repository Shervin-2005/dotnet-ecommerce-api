using Application.DTOs.UserAndAuth;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Domain.Exceptions;

namespace dotnet_ecommerce_api.Controller;

[ApiController]
[Route("api/user-addresses")]
[Authorize]
public class UserAddressesController : ControllerBase
{
    private readonly IUserAddressService _userAddressService;

    public UserAddressesController(IUserAddressService userAddressService)
    {
        _userAddressService = userAddressService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserAddressDto>>> GetAddresses()
    {
        var userId = GetUserId();

        var addresses = 
            await _userAddressService.GetUserAddressesAsync(userId);

        return Ok(addresses);
    }

    [HttpGet("{userAddressId:int}")]
    public async Task<ActionResult<UserAddressDto>> GetAddress(
        int userAddressId)
    {
        var userId = GetUserId();

        var address =
            await _userAddressService.GetByIdAsync(userId, userAddressId);

        return Ok(address);
    }

    [HttpPost]
    public async Task<ActionResult<UserAddressDto>> CreateAddress(
        CreateUserAddressDto dto)
    {
        var userId = GetUserId();

        var address = await _userAddressService.CreateAsync(userId, dto);

        return CreatedAtAction(nameof(GetAddress), new { userAddressId = address.UserAddressId }, address);
    }

    [HttpPut("{userAddressId:int}")]
    public async Task<ActionResult<UserAddressDto>> UpdateAddress(int userAddressId, UpdateUserAddressDto dto)
    {
        var userId = GetUserId();

        var address =
            await _userAddressService.UpdateAsync(userId, userAddressId, dto);

        return Ok(address);
    }

    [HttpDelete("{userAddressId:int}")]
    public async Task<IActionResult> DeleteAddress(int userAddressId)
    {
        var userId = GetUserId();

        await _userAddressService.DeleteAsync(userId, userAddressId);

        return NoContent();
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException("Invalid user identity.");
        }

        return userId;
    }
}