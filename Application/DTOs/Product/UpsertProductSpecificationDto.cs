namespace Application.DTOs.Product;

public class UpsertProductSpecificationDto
{
    public string Key { get; set; } = null!;
    
    public string Value { get; set; } = null!;

    public int DisplayOrder { get; set; }
    
    public bool IsMain { get; set; }
}