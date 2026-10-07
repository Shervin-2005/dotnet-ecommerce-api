namespace Application.DTOs.Product;

public class ProductVariantDto
{
    public int ProductVariantId { get; set; }

    public string Sku { get; set; } = null!;

    public decimal OriginalPrice { get; set; }

    public decimal SalePrice { get; set; }

    public int StockQuantity { get; set; }

    public int SoldQuantity { get; set; }

    public List<ProductAttributeValueDto> AttributeValues { get; set; } = new();
}