using Application.DTOs.OfferCode;

namespace Application.Interfaces;

public interface IOfferCodeService
{
    Task<List<OfferCodeDto>> GetAllAsync();
    Task<OfferCodeDto> GetByIdAsync(int offerCodeId);
    Task<OfferCodeDto> CreateAsync(CreateOfferCodeDto dto);
    Task<OfferCodeDto> UpdateAsync(int offerCodeId, UpdateOfferCodeDto dto);
    Task DeleteAsync(int offerCodeId);
    Task DeactivateAsync(int offerCodeId);
    Task<OfferCodeCalculationResult> CalculateDiscountAsync(string code, int userId, decimal orderAmount);
}