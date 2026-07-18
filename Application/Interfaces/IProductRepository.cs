using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<Product?> GetWithDetailsAsync(int id);
    }
}
