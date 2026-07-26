using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CreateProductDto
    {
        [Required, StringLength(150)]
        public string ProductName { get; set; } = null!;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required, StringLength(2000)]
        public string Description { get; set; } = null!;

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public int CategoryId { get; set; }
        public int BrandId { get; set; }

        public List<ProductImageUploadDto> Images { get; set; } = [];
    }
}
