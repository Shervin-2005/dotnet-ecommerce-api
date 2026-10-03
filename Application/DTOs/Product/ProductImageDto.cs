namespace Application.DTOs.Product;

public class ProductImageDto
{
    public string ImageUrl { get; set; } = null!;
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}