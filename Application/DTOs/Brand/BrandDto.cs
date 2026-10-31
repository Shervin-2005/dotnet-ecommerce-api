namespace Application.DTOs.Brand;

    public class BrandDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;
        public int BrandGroupId { get; set; } 
        public string MainImageUrl { get; set; } = null!;
    }

