namespace Application.DTOs
{
    public class ProductDto
    {
        public int ProductId {  get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
    }
}
