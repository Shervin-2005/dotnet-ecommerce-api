using Application.DTOs.Common;
using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Brand;

public interface IBrandGroupRepository : IGenericRepository<BrandGroup>
{
    Task<PagedResult<BrandGroup>> GetAllWithBrandsAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<BrandGroup?> GetByIdWithBrandsAsync(int id);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
}