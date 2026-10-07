using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductVariantRepository : GenericRepository<ProductVariant>, IProductVariantRepository
{
    public ProductVariantRepository(AppDbContext context) : base(context)
    {
    }
    public async Task<bool> ExistsBySkuAsync(
        string sku,
        int? excludedVariantId = null)
    {
        return await _dbSet.AnyAsync(v =>
            v.Sku == sku &&
            (!excludedVariantId.HasValue ||
             v.ProductVariantId != excludedVariantId.Value));
    }
}