namespace Application.DTOs.Product;

public class ProductAttributeDto
{
    public int ProductAttributeId { get; set; }

    public string Name { get; set; } = null!;

    public List<ProductAttributeValueDto> Values { get; set; } = new();
}