using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.FlashSale;

public interface IFlashSaleRepository : IGenericRepository<Domain.Entities.FlashSale>
{
    Task<Domain.Entities.FlashSale?> GetByIdWithItemsAsync(int flashSaleId, CancellationToken cancellationToken = default);
    Task<List<FlashSaleItem>> GetItemsByFlashSaleIdAsync(int flashSaleId, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingCampaignAsync(
        int productId,
        int? productVariantId,
        DateTime startsAt,
        DateTime endsAt,
        int? excludeFlashSaleId = null,
        CancellationToken cancellationToken = default);
}