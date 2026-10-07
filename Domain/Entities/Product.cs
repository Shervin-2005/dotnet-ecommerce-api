namespace Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public string Description { get; set; } = null!;
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public Guid ImageFolderId { get; set; }

        public int CategoryId { get; set; }
        public int BrandId { get; set; }

        public Category? Category { get; set; }
        public Brand? Brand { get; set; }
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public ICollection<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
        public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();

        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        
    }
}