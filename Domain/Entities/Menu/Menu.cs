namespace Domain.Entities.Menu;

public class Menu
{
    public int MenuId { get; set; }
    public string Name { get; set; } = null!;
    public string Key { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public ICollection<MenuItem> Items { get; set; } = [];
}