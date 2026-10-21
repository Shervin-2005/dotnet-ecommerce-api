namespace dotnet_ecommerce_api.Models;

public class UpdateBrandRequest
{
    public string BrandName { get; set; } = string.Empty;

    public IFormFile? File { get; set; }
}