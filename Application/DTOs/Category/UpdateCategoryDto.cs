namespace Application.DTOs.Category;

public class UpdateCategoryDto
{
    public string CategoryName { get; set; } = null!;
    public Stream? Image { get; set; }
    public string? ImageName { get; set; }
    public string? ContentType { get; set; }
}