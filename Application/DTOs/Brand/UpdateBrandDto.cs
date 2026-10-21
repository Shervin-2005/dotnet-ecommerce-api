namespace Application.DTOs.Brand;

    public class UpdateBrandDto
    {
        public string BrandName { get; set; } = null!;
        public Stream? Image { get; set; }
        public string? ImageName { get; set; }
        public string? ContentType { get; set; }
    }

