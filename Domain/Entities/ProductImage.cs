namespace Domain.Entities
{
    public class ProductImage
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }

        public Product Product { get; set; } = null!;
    }
}
