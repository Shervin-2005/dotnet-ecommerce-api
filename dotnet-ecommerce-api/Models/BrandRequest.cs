using System.ComponentModel.DataAnnotations;

namespace dotnet_ecommerce_api.Models
{
    public class BrandRequest
    {
        [Required, StringLength(150)]
        public string BrandName { get; set; } = null!;

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
