using Application.Interfaces.Product;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductTagRepository : GenericRepository<ProductTag>, IProductTagRepository
{
    public ProductTagRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<List<ProductTag>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _dbSet.Where(t => ids.Contains(t.ProductTagId)).ToListAsync();
    }
    
    public async Task<ProductTag?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<ProductTag?> GetBySlugAsync(string slug)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Slug == slug);
    }
}