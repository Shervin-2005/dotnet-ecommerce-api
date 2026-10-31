namespace Domain.Entities;

public class BrandGroup
{
    public int BrandGroupId { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public ICollection<Brand> Brands { get; set; } = new List<Brand>();
}