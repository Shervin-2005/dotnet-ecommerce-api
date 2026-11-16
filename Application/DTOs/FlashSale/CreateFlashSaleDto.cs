namespace Application.DTOs.FlashSale;

public class CreateFlashSaleDto
{
    public string Title { get; set; } = null!;
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int DisplayOrder { get; set; }
}