using Application.DTOs.Brand;

namespace Application.Interfaces.Brand;

public interface IBrandGroupService
{
    Task<IReadOnlyList<BrandGroupDto>> GetAllWithBrandsAsync();
    Task<BrandGroupDto?> GetByIdAsync(int id);

    Task<BrandGroupDto> CreateAsync(CreateBrandGroupDto dto);

    Task<bool> UpdateAsync(int id, UpdateBrandGroupDto dto);

    Task<bool> DeleteAsync(int id);
}