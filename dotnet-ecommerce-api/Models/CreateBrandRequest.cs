namespace dotnet_ecommerce_api.Models
{
    public class CreateBrandRequest
    {
        public string BrandName { get; set; } = null!;
        
        public IFormFile File { get; set; } = null!;
    }
}
