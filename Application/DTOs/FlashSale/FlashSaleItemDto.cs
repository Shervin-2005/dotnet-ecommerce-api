namespace Application.DTOs.FlashSale;

public class FlashSaleItemDto
{
    public int FlashSaleItemId { get; set; }
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public decimal DiscountPercentage { get; set; }
}