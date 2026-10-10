namespace Domain.Entities;

public class OfferCodeUsage
{
    public int OfferCodeUsageId { get; set; }

    public int OfferCodeId { get; set; }

    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string CodeSnapshot { get; set; } = null!;

    public decimal DiscountAmount { get; set; }

    public DateTime UsedAt { get; set; } = DateTime.UtcNow;

    public OfferCode OfferCode { get; set; } = null!;

    public Order Order { get; set; } = null!;

    public User User { get; set; } = null!;
}