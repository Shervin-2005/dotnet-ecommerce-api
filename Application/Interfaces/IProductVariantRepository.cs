using Domain.Entities;

namespace Application.Interfaces;

public interface IProductVariantRepository : IGenericRepository<ProductVariant>
{
    Task<bool> ExistsBySkuAsync(string sku, int? excludedVariantId = null);
}