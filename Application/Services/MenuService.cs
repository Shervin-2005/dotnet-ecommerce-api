using Application.DTOs.Menu;
using Application.Interfaces.Common;
using Application.Interfaces.Menu;
using AutoMapper;
using Domain.Entities.Menu;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services;

public class MenuService : IMenuService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MenuService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<MenuDto> CreateMenuAsync(CreateMenuDto dto)
    {
        var existingMenu = await _unitOfWork.Menus.GetByKeyAsync(dto.Key);

        if (existingMenu != null)
            throw new ConflictException("A menu with this key already exists.");

        var menu = _mapper.Map<Domain.Entities.Menu.Menu>(dto);

        await _unitOfWork.Menus.AddAsync(menu);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MenuDto>(menu);
    }
    
    public async Task<MenuDto> UpdateMenuAsync(int menuId, UpdateMenuDto dto)
    {
        var menu = await _unitOfWork.Menus.GetByIdAsync(menuId);

        if (menu == null)
            throw new NotFoundException("Menu not found.");

        var existingMenu = await _unitOfWork.Menus.GetByKeyAsync(dto.Key);

        if (existingMenu != null && existingMenu.MenuId != menuId)
            throw new ConflictException("A menu with this key already exists.");

        _mapper.Map(dto, menu);

        _unitOfWork.Menus.Update(menu);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MenuDto>(menu);
    }
    
    public async Task<List<MenuDto>> GetAllMenusAsync()
    {
        var menus = await _unitOfWork.Menus.GetAllAsync();

        return _mapper.Map<List<MenuDto>>(menus);
    }
    
    public async Task<MenuDto?> GetMenuByIdAsync(int menuId)
    {
        var menu = await _unitOfWork.Menus.GetByIdAsync(menuId);

        if (menu == null)
            throw new NotFoundException("Menu not found.");

        var items = await _unitOfWork.MenuItems.GetByMenuIdAsync(menuId);

        var result = _mapper.Map<MenuDto>(menu);
        result.Items = BuildMenuTree(items);

        return result;
    }
    
    public async Task<MenuDto?> GetMenuByKeyAsync(string key)
    {
        var menu = await _unitOfWork.Menus.GetByKeyAsync(key);

        if (menu == null || !menu.IsActive)
            throw new NotFoundException("Menu not found or inactive.");

        var items = await _unitOfWork.MenuItems.GetByMenuIdAsync(menu.MenuId);

        var result = _mapper.Map<MenuDto>(menu);
        result.Items = BuildMenuTree(items, activeOnly: true);

        return result;
    }
    
    public async Task DeleteMenuAsync(int menuId)
    {
        var menu = await _unitOfWork.Menus.GetByIdAsync(menuId);

        if (menu == null)
            throw new NotFoundException("Menu not found.");

        _unitOfWork.Menus.Delete(menu);

        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task<MenuItemDto> CreateMenuItemAsync(int menuId, CreateMenuItemDto dto)
    {
        var menu = await _unitOfWork.Menus.GetByIdAsync(menuId);

        if (menu == null)
            throw new NotFoundException("Menu not found.");

        await ValidateParentAsync(menuId, dto.ParentMenuItemId);
        await ValidateNavigationTargetAsync(dto.Type, dto.CategoryId, dto.BrandId, dto.TagId);

        var menuItem = _mapper.Map<MenuItem>(dto);
        menuItem.MenuId = menuId;

        await _unitOfWork.MenuItems.AddAsync(menuItem);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MenuItemDto>(menuItem);
    }
    
    public async Task<MenuItemDto> UpdateMenuItemAsync(int menuItemId, UpdateMenuItemDto dto)
    {
        var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(menuItemId);

        if (menuItem == null)
            throw new NotFoundException("Menu item not found.");

        var items = await _unitOfWork.MenuItems.GetByMenuIdAsync(menuItem.MenuId);

        await ValidateParentAsync(menuItem.MenuId, dto.ParentMenuItemId);

        if (dto.ParentMenuItemId.HasValue)
            EnsureNoCircularReference(menuItemId, dto.ParentMenuItemId.Value, items);

        await ValidateNavigationTargetAsync(dto.Type, dto.CategoryId, dto.BrandId, dto.TagId);

        _mapper.Map(dto, menuItem);

        _unitOfWork.MenuItems.Update(menuItem);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MenuItemDto>(menuItem);
    }
    
    public async Task DeleteMenuItemAsync(int menuItemId)
    {
        var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(menuItemId);

        if (menuItem == null)
            throw new NotFoundException("Menu item not found.");

        var items = await _unitOfWork.MenuItems.GetByMenuIdAsync(menuItem.MenuId);

        if (items.Any(x => x.ParentMenuItemId == menuItemId))
            throw new ConflictException("Cannot delete a menu item that has children.");
        
        _unitOfWork.MenuItems.Delete(menuItem);

        await _unitOfWork.SaveChangesAsync();
    }
    
    private async Task ValidateParentAsync(int menuId, int? parentMenuItemId)
    {
        if (!parentMenuItemId.HasValue)
            return;

        var parent = await _unitOfWork.MenuItems.GetByIdAsync(parentMenuItemId.Value);

        if (parent == null)
            throw new NotFoundException("Parent menu item not found.");

        if (parent.MenuId != menuId)
            throw new InvalidOperationException("Parent menu item must belong to the same menu.");
    }
    
    private static void EnsureNoCircularReference(int menuItemId, int newParentId,  List<MenuItem> items)
    {
        var itemsById = items.ToDictionary(x => x.MenuItemId);

        var currentId = newParentId;

        while (itemsById.TryGetValue(currentId, out var current))
        {
            if (current.MenuItemId == menuItemId)
                throw new ConflictException("Circular menu hierarchy is not allowed.");

            if (!current.ParentMenuItemId.HasValue)
                break;

            currentId = current.ParentMenuItemId.Value;
        }
    }
    
    private async Task ValidateNavigationTargetAsync(MenuItemType type, int? categoryId, int? BrandId, int? tagId)
    {
        switch (type)
        {
            case MenuItemType.Category:

                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId!.Value);

                if (category == null)
                    throw new NotFoundException("Category not found.");
                
                break;
            
            case MenuItemType.Brand:

                var brand = await _unitOfWork.Brands.GetByIdAsync(BrandId!.Value);

                if (brand == null)
                    throw new NotFoundException("Brand not found.");
                
                break;

            case MenuItemType.Tag:

                var tag = await _unitOfWork.ProductTags.GetByIdAsync(tagId!.Value);
                
                if (tag == null)
                    throw new NotFoundException("Tag not found.");
                
                break;
        }
    }
    
    private List<MenuItemDto> BuildMenuTree(IEnumerable<MenuItem> items, bool activeOnly = false)
    {
        var itemDtos = items
            .Where(x => !activeOnly || x.IsActive)
            .ToDictionary(x => x.MenuItemId, x => _mapper.Map<MenuItemDto>(x));

        var rootItems = new List<MenuItemDto>();

        foreach (var item in items.Where(x => !activeOnly || x.IsActive))
        {
            var dto = itemDtos[item.MenuItemId];

            if (item.ParentMenuItemId == null)
                rootItems.Add(dto);
            
            else if (itemDtos.TryGetValue(item.ParentMenuItemId.Value, out var parent))
                parent.Children.Add(dto);
        }

        SortMenuTree(rootItems);

        return rootItems;
    }
    
    private static void SortMenuTree(List<MenuItemDto> items)
    {
        var sortedItems = items.OrderBy(x => x.DisplayOrder).ToList();

        items.Clear();

        foreach (var item in sortedItems)
        {
            items.Add(item);

            SortMenuTree(item.Children.ToList());
        }
    }
}