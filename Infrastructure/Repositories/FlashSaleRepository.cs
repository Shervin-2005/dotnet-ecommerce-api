using Application.Interfaces.FlashSale;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FlashSaleRepository : GenericRepository<FlashSale>, IFlashSaleRepository
{
    private readonly AppDbContext _context;

    public FlashSaleRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<FlashSale?> GetByIdWithItemsAsync(int flashSaleId, CancellationToken cancellationToken = default)
    {
        return await _context.FlashSales
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .Include(x => x.Items)
                .ThenInclude(x => x.ProductVariant)
            .FirstOrDefaultAsync(
                x => x.FlashSaleId == flashSaleId,
                cancellationToken);
    }

    public async Task<List<FlashSaleItem>> GetItemsByFlashSaleIdAsync(int flashSaleId, CancellationToken cancellationToken = default)
    {
        return await _context.FlashSaleItems
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.ProductVariant)
            .Where(x => x.FlashSaleId == flashSaleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingCampaignAsync(
        int productId,
        int? productVariantId,
        DateTime startsAt,
        DateTime endsAt,
        int? excludeFlashSaleId = null,
        CancellationToken cancellationToken = default)
    {
        return await _context.FlashSaleItems
            .AnyAsync(
                x => x.ProductId == productId
                     && x.ProductVariantId == productVariantId
                     && x.FlashSale.IsEnabled
                     && x.FlashSale.StartsAt < endsAt
                     && x.FlashSale.EndsAt > startsAt
                     && (!excludeFlashSaleId.HasValue || x.FlashSaleId != excludeFlashSaleId.Value),
                cancellationToken);
    }
}