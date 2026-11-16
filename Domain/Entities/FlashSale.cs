namespace Domain.Entities;

public class FlashSale
{
    public int FlashSaleId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public bool IsEnabled { get; set; } = true;

    public int DisplayOrder { get; set; }

    public ICollection<FlashSaleItem> Items { get; set; } = new List<FlashSaleItem>();
}