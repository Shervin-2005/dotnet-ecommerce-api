namespace Application.DTOs.FlashSale;

public class FlashSaleDto
{
    public int FlashSaleId { get; set; }
    public string Title { get; set; } = null!;
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public bool IsEnabled { get; set; }
    public int DisplayOrder { get; set; }

    public List<FlashSaleItemDto> Items { get; set; } = new();
}