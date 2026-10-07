namespace Application.DTOs.Product;

public class CreateProductAttributeDto
{
    public string Name { get; set; } = null!;

    public List<string> Values { get; set; } = new();
}