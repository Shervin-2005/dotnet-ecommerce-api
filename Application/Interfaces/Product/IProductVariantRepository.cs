using Domain.Entities;

namespace Application.Interfaces.Product;

public interface IProductVariantRepository : IGenericRepository<ProductVariant>
{
    Task<bool> ExistsBySkuAsync(string sku, int? excludedVariantId = null);
}