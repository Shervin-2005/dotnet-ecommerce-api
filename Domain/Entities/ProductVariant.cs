namespace Domain.Entities;

public class ProductVariant
{
    public int ProductVariantId { get; set; }

    public int ProductId { get; set; }

    public string Sku { get; set; } = null!; //Stock Keeping Unit

    public decimal OriginalPrice { get; set; }

    public decimal SalePrice { get; set; }

    public int StockQuantity { get; set; }

    public int SoldQuantity { get; set; }

    public Product Product { get; set; } = null!;

    public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
}