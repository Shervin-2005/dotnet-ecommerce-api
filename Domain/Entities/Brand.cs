namespace Domain.Entities
{
    public class Brand
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;
        public string MainImageUrl { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
