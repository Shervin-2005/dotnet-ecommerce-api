namespace Application.DTOs.Product
{
    public class ProductDto
    {
        public int ProductId {  get; set; }
        public string ProductName { get; set; } = null!;
        public decimal OriginalPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public string Description { get; set; } = null!;
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        
        public List<ProductImageDto> Images { get; set; } = null!;
        public List<ProductSpecificationDto> Specifications { get; set; } = new();
        public List<ProductAttributeDto> Attributes { get; set; } = new();
        public List<ProductVariantDto> Variants { get; set; } = new();   
    }
}
