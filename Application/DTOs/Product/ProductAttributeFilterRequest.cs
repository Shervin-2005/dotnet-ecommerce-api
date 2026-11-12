namespace Application.DTOs.Product;

public class ProductAttributeFilterRequest
{
    public string AttributeName { get; set; } = null!;

    public List<string> Values { get; set; } = new();
}