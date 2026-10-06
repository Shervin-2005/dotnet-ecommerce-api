using Application.DTOs.Product;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<bool> UpdateAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AddImageAsync(int id, ProductImageUploadDto uploadDto);
        Task<bool> RemoveImageAsync(int productId, int imageId);
        Task<bool> SetMainImageAsync(int productId, int imageId);
        Task<bool> SetSpecificationsAsync(int productId, List<UpsertProductSpecificationDto> specs);
    }
}
