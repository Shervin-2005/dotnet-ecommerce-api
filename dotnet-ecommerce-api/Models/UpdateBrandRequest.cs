namespace dotnet_ecommerce_api.Models;

public class UpdateBrandRequest
{
    public string BrandName { get; set; } = string.Empty;
    public int? BrandGroupId { get; set; }
    public IFormFile? File { get; set; }
}