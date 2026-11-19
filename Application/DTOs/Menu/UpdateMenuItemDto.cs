using Domain.Enums;

namespace Application.DTOs.Menu;

public class UpdateMenuItemDto
{
    public int? ParentMenuItemId { get; set; }
    public string Title { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public MenuItemType Type { get; set; }
    public string? Url { get; set; }
    public int? CategoryId { get; set; }
    public int? TagId { get; set; }
    public int? BrandId { get; set; }
}