using Domain.Exceptions;

namespace Application.Services;

internal static class SalePricingHelper
{
    public static (decimal SalePrice, DateTime? SaleStartAt, DateTime? SaleEndAt) 
        Configure(decimal originalPrice, decimal? requestedSalePrice, int? saleDurationMinutes)
    {
        var salePrice = requestedSalePrice ?? originalPrice;

        if (salePrice > originalPrice) 
            throw new BadRequestException("Sale price cannot be greater than original price.");

        if (salePrice < originalPrice)
        {
            if (!saleDurationMinutes.HasValue || saleDurationMinutes.Value <= 0)
                throw new BadRequestException("A positive sale duration is required when the sale price is lower than the original price.");
            
            var saleStartAt = DateTime.UtcNow;
            var saleEndAt = saleStartAt.AddMinutes(saleDurationMinutes.Value);

            return (salePrice, saleStartAt, saleEndAt);
        }

        if (saleDurationMinutes.HasValue)
            throw new BadRequestException("A sale duration can only be specified when the sale price is lower than the original price.");
        
        return (originalPrice, null, null);
    }
    
    public static decimal GetEffectivePrice(decimal originalPrice, decimal salePrice, DateTime? saleStartAt, DateTime? saleEndAt, DateTime utcNow)
    {
        var isSaleActive =
            saleStartAt.HasValue &&
            saleEndAt.HasValue &&
            saleStartAt.Value <= utcNow &&
            utcNow < saleEndAt.Value &&
            salePrice < originalPrice;

        return isSaleActive ? salePrice : originalPrice;
    }
}