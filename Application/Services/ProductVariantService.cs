using Application.DTOs.Product;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services;

public class ProductVariantService : IProductVariantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductVariantService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductVariantDto> CreateAsync(int productId, CreateProductVariantDto dto)
    {
        var product = await _unitOfWork.Products.GetWithDetailsAsync(productId);

        if (product is null)
            throw new BadRequestException("Product not found.");

        var attributeValues = product.Attributes
            .SelectMany(a => a.Values)
            .Where(v => dto.AttributeValueIds.Contains(
                v.ProductAttributeValueId))
            .ToList();

        if (attributeValues.Count != dto.AttributeValueIds.Count) 
            throw new BadRequestException("One or more attribute values do not belong to this product.");
        
        ValidateAttributeCombination(attributeValues);

        if (await SkuExistsAsync(dto.Sku.Trim()))
            throw new BadRequestException("SKU already exists.");

        if (VariantCombinationExists(product.Variants, attributeValues)) 
            throw new BadRequestException("This variant combination already exists.");

        var variant = new ProductVariant
        {
            ProductId = productId,
            Sku = dto.Sku.Trim(),
            OriginalPrice = dto.OriginalPrice,
            SalePrice = dto.SalePrice ?? dto.OriginalPrice,
            StockQuantity = dto.StockQuantity
        };

        foreach (var attributeValue in attributeValues)
        {
            variant.AttributeValues.Add(attributeValue);
        }

        await _unitOfWork.ProductVariants.AddAsync(variant);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductVariantDto>(variant);
    }

    public async Task<ProductVariantDto?> GetByIdAsync(int productId, int variantId)
    {
        var product = await _unitOfWork.Products.GetWithDetailsAsync(productId);

        if (product is null) return null;

        var variant = product.Variants.FirstOrDefault(v => v.ProductVariantId == variantId);

        return variant is null ? null : _mapper.Map<ProductVariantDto>(variant);
    }

    public async Task<bool> UpdateAsync(int productId, int variantId, UpdateProductVariantDto dto)
    {
        var product = await _unitOfWork.Products.GetWithDetailsAsync(productId);

        if (product is null)
            return false;

        var variant = product.Variants.FirstOrDefault(v => v.ProductVariantId == variantId);

        if (variant is null)
            return false;

        var attributeValues = product.Attributes
            .SelectMany(a => a.Values)
            .Where(v => dto.AttributeValueIds.Contains(
                v.ProductAttributeValueId))
            .ToList();

        if (attributeValues.Count != dto.AttributeValueIds.Count)
            throw new BadRequestException("One or more attribute values do not belong to this product.");
        
        ValidateAttributeCombination(attributeValues);

        var sku = dto.Sku.Trim();

        if (await SkuExistsAsync(sku, variantId)) throw new BadRequestException("SKU already exists.");
        

        var otherVariants = product.Variants
            .Where(v => v.ProductVariantId != variantId);

        if (VariantCombinationExists(otherVariants, attributeValues))
            throw new BadRequestException("This variant combination already exists.");
        
        variant.Sku = sku;
        variant.OriginalPrice = dto.OriginalPrice;
        variant.SalePrice = dto.SalePrice ?? dto.OriginalPrice;
        variant.StockQuantity = dto.StockQuantity;

        variant.AttributeValues.Clear();

        foreach (var attributeValue in attributeValues)
        {
            variant.AttributeValues.Add(attributeValue);
        }

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
    public async Task<bool> DeleteAsync(int productId, int variantId)
    {
        var product = await _unitOfWork.Products.GetWithDetailsAsync(productId);

        if (product is null) return false;

        var variant = product.Variants
            .FirstOrDefault(v =>
                v.ProductVariantId == variantId);

        if (variant is null) return false;

        _unitOfWork.ProductVariants.Delete(variant);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static void ValidateAttributeCombination(List<ProductAttributeValue> values)
    {
        var attributeCount = values
            .Select(v => v.ProductAttributeId)
            .Distinct()
            .Count();

        if (attributeCount != values.Count)
            throw new BadRequestException("A variant cannot contain multiple values from the same attribute.");
    }

    private static bool VariantCombinationExists(IEnumerable<ProductVariant> variants, List<ProductAttributeValue> attributeValues)
    {
        var requestedIds = attributeValues
            .Select(v => v.ProductAttributeValueId)
            .OrderBy(id => id)
            .ToList();

        return variants.Any(variant =>
        {
            var existingIds = variant.AttributeValues
                .Select(v => v.ProductAttributeValueId)
                .OrderBy(id => id)
                .ToList();

            return existingIds.SequenceEqual(requestedIds);
        });
    }

    private async Task<bool> SkuExistsAsync(string sku, int? excludedVariantId = null)
    {
        return await _unitOfWork.ProductVariants
            .ExistsBySkuAsync(sku, excludedVariantId);
    }
}