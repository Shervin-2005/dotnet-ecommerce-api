namespace Application.DTOs.Brand
{
    public class CreateBrandDto
    {
        public string BrandName { get; set; } = null!;
        
        public Stream Image { get; set; } = default!;
        
        public string ImageName { get; set; } = null!;
        
        public string ContentType { get; set; } = null!;
    }
}
