namespace Application.DTOs.Category;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? MainImageUrl { get; set; }
}