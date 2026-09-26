namespace dotnet_ecommerce_api.Models
{
    public class BrandRequest
    {
        public string BrandName { get; set; } = null!;
        
        public IFormFile File { get; set; } = null!;
    }
}
