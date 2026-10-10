namespace Application.DTOs.OfferCode;

public class CreateOfferCodeDto
{
    public string Code { get; set; } = null!;

    public decimal DiscountPercentage { get; set; }

    public decimal? MinimumOrderAmount { get; set; }

    public decimal? MaximumDiscountAmount { get; set; }

    public int? UsageLimit { get; set; }

    public int? UserUsageLimit { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime ExpiresAt { get; set; }
}