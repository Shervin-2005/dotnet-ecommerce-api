using Application.DTOs.Brand;
using Application.DTOs.Common;

namespace Application.Interfaces.Brand
{
    public interface IBrandService
    {
        Task<PagedResult<BrandDto>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<BrandDto?> GetByIdAsync(int id);
        Task<BrandDto> CreateAsync(CreateBrandDto dto);
        Task<bool> UpdateAsync(int id, UpdateBrandDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
