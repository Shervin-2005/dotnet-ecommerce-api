namespace Domain.Entities;

public class ProductSpecification
{
    public int ProductSpecificationId { get; set; }
    public int ProductId { get; set; }
    public string Key { get; set; } = null!;   
    public string Value { get; set; } = null!; 
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }

    public Product Product { get; set; } = null!;
}