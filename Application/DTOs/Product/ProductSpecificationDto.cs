namespace Application.DTOs.Product;

public class ProductSpecificationDto
{
    public int ProductSpecificationId { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}