namespace Application.DTOs.OfferCode;

public class UpdateOfferCodeDto
{
    public decimal DiscountPercentage { get; set; }

    public decimal? MinimumOrderAmount { get; set; }

    public decimal? MaximumDiscountAmount { get; set; }

    public int? UsageLimit { get; set; }

    public int? UserUsageLimit { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsActive { get; set; }
}