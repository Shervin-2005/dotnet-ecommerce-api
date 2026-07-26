using System.ComponentModel.DataAnnotations;

namespace dotnet_ecommerce_api.Models
{
    public class ProductRequest
    {
        [Required, StringLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public List<IFormFile> Images { get; set; } = [];
    }
}
