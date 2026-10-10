namespace Application.DTOs.OfferCode;

public class OfferCodeCalculationResult
{
    public int OfferCodeId { get; set; }

    public string Code { get; set; } = null!;

    public decimal DiscountAmount { get; set; }
}