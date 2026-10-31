using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Brand;

public interface IBrandGroupRepository : IGenericRepository<BrandGroup>
{
    Task<IReadOnlyList<BrandGroup>> GetAllWithBrandsAsync();
    
    Task<BrandGroup?> GetByIdWithBrandsAsync(int id);

    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
}