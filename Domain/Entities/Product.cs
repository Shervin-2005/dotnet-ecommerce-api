namespace Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }

        public int CategoryId { get; set; }
        public int BrandId { get; set; }

        public Category? Category { get; set; }
        public Brand? Brand { get; set; }
    }
}