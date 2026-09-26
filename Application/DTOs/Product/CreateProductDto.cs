namespace Application.DTOs.Product;

public class CreateProductDto
{
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public string Description { get; set; } = null!;
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public List<ProductImageDto> Images { get; set; } = [];
}