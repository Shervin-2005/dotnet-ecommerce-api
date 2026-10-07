using Application.DTOs.Product;

namespace Application.Interfaces;

public interface IProductVariantService
{
    Task<ProductVariantDto?> GetByIdAsync(int productId, int variantId);

    Task<ProductVariantDto> CreateAsync(
        int productId,
        CreateProductVariantDto dto);

    Task<bool> UpdateAsync(
        int productId,
        int variantId,
        UpdateProductVariantDto dto);

    Task<bool> DeleteAsync(
        int productId,
        int variantId);
}