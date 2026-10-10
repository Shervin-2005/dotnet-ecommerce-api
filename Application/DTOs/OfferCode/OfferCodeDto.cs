namespace Application.DTOs.OfferCode;

public class OfferCodeDto
{
    public int OfferCodeId { get; set; }

    public string Code { get; set; } = null!;

    public decimal DiscountPercentage { get; set; }

    public decimal? MinimumOrderAmount { get; set; }

    public decimal? MaximumDiscountAmount { get; set; }

    public int? UsageLimit { get; set; }

    public int UsedCount { get; set; }

    public int? UserUsageLimit { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}