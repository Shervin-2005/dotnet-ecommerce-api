using Application.DTOs.Common;
using Application.DTOs.OfferCode;

namespace Application.Interfaces.OfferCode;

public interface IOfferCodeService
{
    Task<PagedResult<OfferCodeDto>> GetAllAsync(PaginationRequest request, CancellationToken cancellationToken = default);    
    Task<OfferCodeDto> GetByIdAsync(int offerCodeId);
    Task<OfferCodeDto> CreateAsync(CreateOfferCodeDto dto);
    Task<OfferCodeDto> UpdateAsync(int offerCodeId, UpdateOfferCodeDto dto);
    Task DeleteAsync(int offerCodeId);
    Task DeactivateAsync(int offerCodeId);
    Task<OfferCodeCalculationResult> CalculateDiscountAsync(string code, int userId, decimal orderAmount);
}