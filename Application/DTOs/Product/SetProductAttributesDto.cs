namespace Application.DTOs.Product;

public class SetProductAttributesDto
{
    public List<CreateProductAttributeDto> Attributes { get; set; } = new();
}