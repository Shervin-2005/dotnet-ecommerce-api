using Application.DTOs.Category;
using Application.DTOs.Common;

namespace Application.Interfaces.Category
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryTreeDto>> GetTreeAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<bool> UpdateAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
