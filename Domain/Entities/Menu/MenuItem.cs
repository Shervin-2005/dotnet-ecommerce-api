using Domain.Enums;

namespace Domain.Entities.Menu;

public class MenuItem
{
    public int MenuItemId { get; set; }
    public int MenuId { get; set; }
    public int? ParentMenuItemId { get; set; }
    public string Title { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public MenuItemType Type { get; set; }
    public string? Url { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? TagId { get; set; }

    public Menu Menu { get; set; } = null!;
    public MenuItem? Parent { get; set; }
    public ICollection<MenuItem> Children { get; set; } = [];
}