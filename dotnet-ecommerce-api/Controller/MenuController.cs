using Application.DTOs.Menu;
using Application.Interfaces.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller;

[ApiController]
public class MenuController : BaseController
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    // Menu endpoints

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto dto)
    {
        var result = await _menuService.CreateMenuAsync(dto);

        return CreatedAtAction(nameof(GetMenuById), new { menuId = result.MenuId }, result);
    }

    [HttpPut("{menuId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMenu(int menuId, [FromBody] UpdateMenuDto dto)
    {
        var result = await _menuService.UpdateMenuAsync(menuId, dto);
        
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllMenus()
    {
        var result = await _menuService.GetAllMenusAsync();

        return Ok(result);
    }

    [HttpGet("{menuId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetMenuById(int menuId)
    {
        var result = await _menuService.GetMenuByIdAsync(menuId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{key}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMenuByKey(string key)
    {
        var result = await _menuService.GetMenuByKeyAsync(key);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{menuId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMenu(int menuId)
    {
        await _menuService.DeleteMenuAsync(menuId);

        return NoContent();
    }

    [HttpPost("{menuId:int}/items")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateMenuItem(int menuId, [FromBody] CreateMenuItemDto dto)
    {
        var result = await _menuService.CreateMenuItemAsync(menuId, dto);

        return Ok(result);
    }

    [HttpPut("items/{menuItemId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMenuItem(int menuItemId, [FromBody] UpdateMenuItemDto dto)
    {
        var result = await _menuService.UpdateMenuItemAsync(menuItemId, dto);

        return Ok(result);
    }

    [HttpDelete("items/{menuItemId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMenuItem(int menuItemId)
    {
        await _menuService.DeleteMenuItemAsync(menuItemId);

        return NoContent();
    }
}