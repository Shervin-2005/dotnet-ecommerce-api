namespace Application.DTOs.FlashSale;

public class AddFlashSaleItemDto
{
    public int ProductId { get; set; }
    public List<int> ProductVariantIds { get; set; } = new();
    public decimal DiscountPercentage { get; set; }
}