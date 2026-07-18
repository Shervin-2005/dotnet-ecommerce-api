using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId) =>
            await _dbSet.Where(p => p.CategoryId == categoryId).ToListAsync();

        public async Task<Product?> GetWithDetailsAsync(int id) =>
            await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.ProductId == id);
    }
}
