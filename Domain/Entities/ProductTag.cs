namespace Domain.Entities;

public class ProductTag
{
    public int ProductTagId { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}