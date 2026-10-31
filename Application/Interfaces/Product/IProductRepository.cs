using Application.DTOs.Common;
using Application.DTOs.Product;

namespace Application.Interfaces.Product
{
    public interface IProductRepository : IGenericRepository<Domain.Entities.Product>
    {
        Task<IEnumerable<Domain.Entities.Product>> GetByCategoryAsync(int categoryId);
        Task<Domain.Entities.Product?> GetWithDetailsAsync(int id);
        Task<IEnumerable<Domain.Entities.Product>> GetAllWithDetailsAsync();
        Task<PagedResult<Domain.Entities.Product>> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Domain.Entities.Product>> GetSimilarProductsAsync(int productId, int count = 8, CancellationToken cancellationToken = default);
    }
}
