using Application.DTOs.FlashSale;
using Application.Interfaces.Common;
using Application.Interfaces.FlashSale;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services;

public class FlashSaleService : IFlashSaleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;


    public FlashSaleService(IUnitOfWork unitOfWork,  IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<FlashSaleDto> CreateAsync(CreateFlashSaleDto dto, CancellationToken cancellationToken = default)
    {
        var flashSale = _mapper.Map<FlashSale>(dto);

        flashSale.IsEnabled = true;

        await _unitOfWork.FlashSales.AddAsync(flashSale);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FlashSaleDto>(flashSale);
    }
    
    public async Task UpdateAsync(int flashSaleId, UpdateFlashSaleDto dto, CancellationToken cancellationToken = default)
    {
        var flashSale = await _unitOfWork.FlashSales.GetByIdWithItemsAsync(flashSaleId, cancellationToken);

        if (flashSale == null)
            throw new NotFoundException("Flash Sale campaign not found.");

        if (dto.IsEnabled)
        {
            foreach (var item in flashSale.Items)
            {
                if (HasIndividualSaleOverlap(item.Product, item.ProductVariant, dto.StartsAt, dto.EndsAt))
                {
                    throw new ConflictException(
                        "An individual sale conflicts with this Flash Sale.");
                }

                var hasOverlap = await _unitOfWork.FlashSales
                    .HasOverlappingCampaignAsync(
                        item.ProductId,
                        item.ProductVariantId,
                        dto.StartsAt,
                        dto.EndsAt,
                        flashSaleId,
                        cancellationToken);

                if (hasOverlap)
                    throw new ConflictException("This product or variant already belongs to an overlapping Flash Sale.");
            }
        }
        
        _mapper.Map(dto, flashSale);
        
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task<FlashSaleDto?> GetByIdAsync(int flashSaleId, CancellationToken cancellationToken = default)
    {
        var flashSale = await _unitOfWork.FlashSales.GetByIdWithItemsAsync(flashSaleId, cancellationToken);

        return _mapper.Map<FlashSaleDto>(flashSale);
    }
    
    public async Task AddItemsAsync(int flashSaleId, AddFlashSaleItemDto dto, CancellationToken cancellationToken = default)
    {
        var flashSale = await _unitOfWork.FlashSales.GetByIdWithItemsAsync(flashSaleId, cancellationToken);

        if (flashSale == null)
            throw new NotFoundException("Flash Sale campaign not found.");

        var product = await _unitOfWork.Products.GetByIdWithVariantsAsync(dto.ProductId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product not found.");

        // Product without variants
        if (!product.Variants.Any())
        {
            if (dto.ProductVariantIds.Any())
            {
                throw new BadRequestException("This product does not have any variants.");
            }

            await EnsureItemCanBeAddedAsync(flashSale, product, null, cancellationToken);

            flashSale.Items.Add(new FlashSaleItem
            {
                ProductId = product.ProductId,
                ProductVariantId = null,
                DiscountPercentage = dto.DiscountPercentage
            });
        }
        // Product with variants
        else
        {
            if (!dto.ProductVariantIds.Any())
                throw new BadRequestException("Select at least one variant for this product.");
            
            var selectedVariants = product.Variants.Where(v => dto.ProductVariantIds.Contains(v.ProductVariantId)).ToList();

            if (selectedVariants.Count != dto.ProductVariantIds.Count)
                throw new BadRequestException("One or more selected variants do not belong to this product.");
            

            foreach (var variant in selectedVariants)
            {
                await EnsureItemCanBeAddedAsync(flashSale, product, variant, cancellationToken);

                flashSale.Items.Add(new FlashSaleItem
                {
                    ProductId = product.ProductId,
                    ProductVariantId = variant.ProductVariantId,
                    DiscountPercentage = dto.DiscountPercentage
                });
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task RemoveItemAsync(int flashSaleId, int flashSaleItemId, CancellationToken cancellationToken = default)
    {
        var flashSale = await _unitOfWork.FlashSales.GetByIdWithItemsAsync(flashSaleId, cancellationToken);

        if (flashSale == null)
            throw new NotFoundException("Flash Sale campaign not found.");

        var item = flashSale.Items.FirstOrDefault(x => x.FlashSaleItemId == flashSaleItemId);

        if (item == null)
            throw new NotFoundException("Flash Sale item not found.");

        flashSale.Items.Remove(item);

        await _unitOfWork.SaveChangesAsync();
    }
    
    private async Task EnsureItemCanBeAddedAsync(FlashSale flashSale, Product product, ProductVariant? variant, CancellationToken cancellationToken)
    {
        var variantId = variant?.ProductVariantId;

        // Prevent duplicate items in the same campaign
        var alreadyExists = flashSale.Items.Any(x =>
            x.ProductId == product.ProductId &&
            x.ProductVariantId == variantId);

        if (alreadyExists)
            throw new BadRequestException("This product or variant already exists in this campaign.");
        
        // Prevent conflicts with individual sales
        if (HasIndividualSaleOverlap(product, variant, flashSale.StartsAt, flashSale.EndsAt))
            throw new ConflictException("An individual sale conflicts with this Flash Sale.");
        

        // Prevent overlapping Flash Sale campaigns
        var hasOverlap = await _unitOfWork.FlashSales
            .HasOverlappingCampaignAsync(
                product.ProductId,
                variantId,
                flashSale.StartsAt,
                flashSale.EndsAt,
                cancellationToken: cancellationToken);

        if (hasOverlap)
            throw new BadRequestException("This product or variant already belongs to an overlapping Flash Sale.");
    }
    
    private static bool HasIndividualSaleOverlap(Product product, ProductVariant? variant, DateTime campaignStartsAt, DateTime campaignEndsAt)
    {
        var originalPrice = variant is null ? product.OriginalPrice : variant.OriginalPrice;
        var salePrice = variant is null ? product.SalePrice : variant.SalePrice;
        var saleStartsAt = variant is null ? product.SaleStartAt : variant.SaleStartAt;
        var saleEndsAt = variant is null ? product.SaleEndAt : variant.SaleEndAt;

        if (salePrice >= originalPrice)
            return false;

        return (!saleStartsAt.HasValue || saleStartsAt.Value < campaignEndsAt)
               &&
               (!saleEndsAt.HasValue || saleEndsAt.Value > campaignStartsAt);
    }
}