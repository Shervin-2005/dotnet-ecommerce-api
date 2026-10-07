namespace Domain.Entities;

public class ProductAttributeValue
{
    public int ProductAttributeValueId { get; set; }

    public int ProductAttributeId { get; set; }

    public string Value { get; set; } = null!;

    public ProductAttribute ProductAttribute { get; set; } = null!;

    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}