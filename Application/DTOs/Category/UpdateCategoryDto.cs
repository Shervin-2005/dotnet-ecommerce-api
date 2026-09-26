namespace Application.DTOs.Category;

public class UpdateCategoryDto
{
    public string CategoryName { get; set; } = null!;
    public Stream Image { get; set; } = default!;
    public string ImageName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}