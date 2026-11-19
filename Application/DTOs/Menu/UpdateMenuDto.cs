namespace Application.DTOs.Menu;

public class UpdateMenuDto  
{
    public string Name { get; set; } = null!;
    public string Key { get; set; } = null!;
    public bool IsActive { get; set; }
}