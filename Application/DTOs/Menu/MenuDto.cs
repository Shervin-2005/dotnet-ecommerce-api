namespace Application.DTOs.Menu;

public class MenuDto
{
    public int MenuId { get; set; }
    public string Name { get; set; } = null!;
    public string Key { get; set; } = null!;
    public bool IsActive { get; set; }
    public ICollection<MenuItemDto> Items { get; set; } = [];
}