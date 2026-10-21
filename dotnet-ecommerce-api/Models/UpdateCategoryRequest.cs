namespace dotnet_ecommerce_api.Models;

public class UpdateCategoryRequest
{
    public string CategoryName { get; set; } = string.Empty;
    public IFormFile? File { get; set; }
}