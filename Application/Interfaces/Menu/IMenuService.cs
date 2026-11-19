using Application.DTOs.Menu;

namespace Application.Interfaces.Menu;

public interface IMenuService
{
    Task<MenuDto> CreateMenuAsync(CreateMenuDto dto);
    Task<MenuDto> UpdateMenuAsync(int menuId, UpdateMenuDto dto);
    Task<List<MenuDto>> GetAllMenusAsync();
    Task<MenuDto?> GetMenuByIdAsync(int menuId);
    Task<MenuDto?> GetMenuByKeyAsync(string key);
    Task DeleteMenuAsync(int menuId);
    Task<MenuItemDto> CreateMenuItemAsync(int menuId, CreateMenuItemDto dto);
    Task<MenuItemDto> UpdateMenuItemAsync(int menuItemId, UpdateMenuItemDto dto);
    Task DeleteMenuItemAsync(int menuItemId);
}