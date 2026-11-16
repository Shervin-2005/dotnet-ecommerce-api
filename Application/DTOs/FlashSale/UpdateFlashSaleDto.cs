namespace Application.DTOs.FlashSale;

public class UpdateFlashSaleDto
{
    public string Title { get; set; } = null!;
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsEnabled { get; set; }
}