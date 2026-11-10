namespace Application.DTOs.Category;

public class CategoryTreeDto
{ 
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!; 
    public string MainImageUrl { get; set; } = null!;
    
    public List<CategoryTreeDto> SubCategories { get; set; } = new();
}