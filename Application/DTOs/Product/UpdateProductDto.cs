namespace Application.DTOs.Product;

public class UpdateProductDto
{
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public string Description { get; set; } = null!;
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
}