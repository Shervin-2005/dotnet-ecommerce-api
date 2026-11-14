namespace Application.DTOs.Product;

public class ProductTagDto
{
    public int ProductTagId { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;
}