namespace Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string MainImageUrl { get; set; } = null!;
        public Guid ImageFolderId { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
