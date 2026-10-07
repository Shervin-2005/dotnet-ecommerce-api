namespace Application.DTOs.Product;

public class UpdateProductVariantDto
{
    public string Sku { get; set; } = null!;

    public decimal OriginalPrice { get; set; }

    public decimal? SalePrice { get; set; }

    public int StockQuantity { get; set; }

    public List<int> AttributeValueIds { get; set; } = new();
}