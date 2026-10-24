namespace Application.DTOs.Product;

public class ProductImageDto
{
    public int ImageId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
}