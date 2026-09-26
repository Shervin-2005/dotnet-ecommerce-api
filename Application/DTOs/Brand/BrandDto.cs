namespace Application.DTOs.Brand;

    public class BrandDto
    {
        public int BrandId { get; set; }
        
        public string BrandName { get; set; } = null!;
        
        public string MainImageUrl { get; set; } = null!;
    }

