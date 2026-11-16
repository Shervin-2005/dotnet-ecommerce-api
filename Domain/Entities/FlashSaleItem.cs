namespace Domain.Entities;

public class FlashSaleItem
{
    public int FlashSaleItemId { get; set; }

    public int FlashSaleId { get; set; }

    public int ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    public decimal DiscountPercentage { get; set; }

    public FlashSale FlashSale { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public ProductVariant? ProductVariant { get; set; }
}