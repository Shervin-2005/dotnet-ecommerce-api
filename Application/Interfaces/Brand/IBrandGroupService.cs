using Application.DTOs.Brand;
using Application.DTOs.Common;

namespace Application.Interfaces.Brand;

public interface IBrandGroupService
{
    Task<PagedResult<BrandGroupDto>> GetAllWithBrandsAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<BrandGroupDto?> GetByIdAsync(int id);
    Task<BrandGroupDto> CreateAsync(CreateBrandGroupDto dto);
    Task<bool> UpdateAsync(int id, UpdateBrandGroupDto dto);
    Task<bool> DeleteAsync(int id);
}