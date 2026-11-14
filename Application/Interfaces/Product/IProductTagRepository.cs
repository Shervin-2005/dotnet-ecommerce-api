using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Product;

public interface IProductTagRepository : IGenericRepository<ProductTag>
{
    Task<List<ProductTag>> GetByIdsAsync(IEnumerable<int> ids);
    Task<ProductTag?> GetByNameAsync(string name);
    Task<ProductTag?> GetBySlugAsync(string slug);
}