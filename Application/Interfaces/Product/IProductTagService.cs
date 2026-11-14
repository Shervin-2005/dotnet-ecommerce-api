using Application.DTOs.Product;

namespace Application.Interfaces.Product;

public interface IProductTagService
{
    Task<IEnumerable<ProductTagDto>> GetAllAsync();
    Task<ProductTagDto?> GetByIdAsync(int id);
    Task<ProductTagDto> CreateAsync(CreateProductTagDto dto);
    Task<bool> UpdateAsync(int id, UpdateProductTagDto dto);
    Task<bool> DeleteAsync(int id);
}