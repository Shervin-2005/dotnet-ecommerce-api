namespace Application.DTOs.Brand;

public class BrandGroupDto
{
    public int BrandGroupId { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int DisplayOrder { get; set; }

    public ICollection<BrandDto> Brands { get; set; } = new List<BrandDto>();
}