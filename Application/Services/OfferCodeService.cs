using Application.DTOs.OfferCode;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class OfferCodeService : IOfferCodeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OfferCodeService> _logger;

    public OfferCodeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OfferCodeService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<OfferCodeDto>> GetAllAsync()
    {
        var offerCodes = await _unitOfWork.OfferCodes.GetAllAsync();

        return _mapper.Map<List<OfferCodeDto>>(offerCodes);
    }

    public async Task<OfferCodeDto> GetByIdAsync(int offerCodeId)
    {
        var offerCode = await _unitOfWork.OfferCodes.GetByIdAsync(offerCodeId);

        if (offerCode is null)
            throw new NotFoundException("Offer code not found.");

        return _mapper.Map<OfferCodeDto>(offerCode);
    }

    public async Task<OfferCodeDto> CreateAsync(
        CreateOfferCodeDto dto)
    {
        var normalizedCode = dto.Code.Trim().ToUpperInvariant();
        
        var existing = await _unitOfWork.OfferCodes.GetByCodeAsync(normalizedCode);

        if (existing is not null)
            throw new BadRequestException("An offer code with this code already exists.");

        var offerCode = _mapper.Map<OfferCode>(dto);

        offerCode.Code = normalizedCode;
        offerCode.UsedCount = 0;
        offerCode.IsActive = true;
        offerCode.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.OfferCodes.AddAsync(offerCode);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Offer code {Code} was created.", offerCode.Code);

        return _mapper.Map<OfferCodeDto>(offerCode);
    }

    public async Task<OfferCodeDto> UpdateAsync(int offerCodeId, UpdateOfferCodeDto dto)
    {
        var offerCode = await _unitOfWork.OfferCodes.GetByIdAsync(offerCodeId);

        if (offerCode is null)
            throw new NotFoundException("Offer code not found.");

        if (dto.UsageLimit.HasValue && dto.UsageLimit.Value < offerCode.UsedCount)
        {
            throw new BadRequestException("Usage limit cannot be lower than the number of times this offer code has already been used.");
        }

        _mapper.Map(dto, offerCode);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Offer code {OfferCodeId} was updated.", offerCodeId);

        return _mapper.Map<OfferCodeDto>(offerCode);
    }

    public async Task DeleteAsync(int offerCodeId)
    {
        var offerCode = await _unitOfWork.OfferCodes.GetByIdAsync(offerCodeId);

        if (offerCode is null)
            throw new NotFoundException("Offer code not found.");

        if (offerCode.UsedCount > 0)
            throw new BadRequestException("This offer code has usage history and cannot be deleted. Deactivate it instead.");
        

        _unitOfWork.OfferCodes.Delete(offerCode);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Offer code {OfferCodeId} was deleted.", offerCodeId);
    }
    
    public async Task DeactivateAsync(int offerCodeId)
    {
        var offerCode = await _unitOfWork.OfferCodes
            .GetByIdAsync(offerCodeId);

        if (offerCode is null)
            throw new NotFoundException("Offer code not found.");

        if (!offerCode.IsActive)
            return;

        offerCode.IsActive = false;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Offer code {OfferCodeId} was deactivated.", offerCodeId);
    }

    public async Task<OfferCodeCalculationResult> CalculateDiscountAsync(string code, int userId, decimal orderAmount)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();

        var offerCode = await _unitOfWork.OfferCodes.GetByCodeAsync(normalizedCode);

        if (offerCode is null) 
            throw new BadRequestException("Invalid offer code.");

        var now = DateTime.UtcNow;

        if (!offerCode.IsActive)
            throw new BadRequestException("This offer code is no longer active.");

        if (now < offerCode.StartsAt)
            throw new BadRequestException("This offer code is not active yet.");

        if (now >= offerCode.ExpiresAt)
            throw new BadRequestException("This offer code has expired.");

        if (offerCode.UsageLimit.HasValue && offerCode.UsedCount >= offerCode.UsageLimit.Value)
        {
            throw new BadRequestException("This offer code has reached its usage limit.");
        }

        if (offerCode.UserUsageLimit.HasValue)
        {
            var userUsageCount = await _unitOfWork.OfferCodes.GetUserUsageCountAsync(offerCode.OfferCodeId, userId);

            if (userUsageCount >= offerCode.UserUsageLimit.Value)
                throw new BadRequestException("You have reached the usage limit for this offer code.");
            
        }

        if (offerCode.MinimumOrderAmount.HasValue && orderAmount < offerCode.MinimumOrderAmount.Value)
            throw new BadRequestException($"This offer code requires a minimum order amount of {offerCode.MinimumOrderAmount.Value}.");
        

        var discountAmount = orderAmount * offerCode.DiscountPercentage / 100m;

        if (offerCode.MaximumDiscountAmount.HasValue)
            discountAmount = Math.Min(discountAmount, offerCode.MaximumDiscountAmount.Value);
        

        discountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero);

        return new OfferCodeCalculationResult
        {
            OfferCodeId = offerCode.OfferCodeId,
            Code = offerCode.Code,
            DiscountAmount = discountAmount
        };
    }
}