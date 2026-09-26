namespace dotnet_ecommerce_api.Models;

public class ProductRequest
{
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public List<IFormFile> Images { get; set; } = [];
}