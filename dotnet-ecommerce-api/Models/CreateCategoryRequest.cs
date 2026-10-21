namespace dotnet_ecommerce_api.Models;

public class CreateCategoryRequest
{
    public string CategoryName { get; set; } = null!;
    public IFormFile File { get; set; } = null!;
}