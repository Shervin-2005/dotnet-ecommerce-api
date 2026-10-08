using Application.DTOs.UserAndAuth;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services;

public class UserAddressService : IUserAddressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserAddressService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<UserAddressDto>> GetUserAddressesAsync(int userId)
    {
        var addresses =
            await _unitOfWork.UserAddresses.GetUserAddressesAsync(userId);

        return _mapper.Map<List<UserAddressDto>>(addresses);
    }

    public async Task<UserAddressDto> GetByIdAsync(int userId, int userAddressId)
    {
        var address =
            await _unitOfWork.UserAddresses.GetByIdAndUserIdAsync(userAddressId, userId);

        if (address is null)
            throw new NotFoundException("Address not found.");

        return _mapper.Map<UserAddressDto>(address);
    }

    public async Task<UserAddressDto> CreateAsync(int userId, CreateUserAddressDto dto)
    {
        var address = _mapper.Map<UserAddress>(dto);

        address.UserId = userId;

        if (dto.IsDefault)
        {
            await ClearDefaultAddressAsync(userId);
        }
        await _unitOfWork.UserAddresses.AddAsync(address);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserAddressDto>(address);
    }

    public async Task<UserAddressDto> UpdateAsync(int userId, int userAddressId, UpdateUserAddressDto dto)
    {
        var address =
            await _unitOfWork.UserAddresses.GetByIdAndUserIdAsync(userAddressId, userId);

        if (address is null)
            throw new NotFoundException("Address not found.");

        if (dto.IsDefault && !address.IsDefault)
        {
            await ClearDefaultAddressAsync(userId);
        }

        _mapper.Map(dto, address);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserAddressDto>(address);
    }

    public async Task DeleteAsync(int userId, int userAddressId)
    {
        var address =
            await _unitOfWork.UserAddresses.GetByIdAndUserIdAsync(userAddressId, userId);

        if (address is null)
            throw new NotFoundException("Address not found.");

        if (address.IsDefault)
        {
            var nextDefault =
                await _unitOfWork.UserAddresses
                    .GetNextDefaultCandidateAsync(userId, userAddressId);

            address.IsDefault = false;

            if (nextDefault is not null)
            {
                nextDefault.IsDefault = true;
            }
        }

        _unitOfWork.UserAddresses.Delete(address);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ClearDefaultAddressAsync(int userId)
    {
        var currentDefault =
            await _unitOfWork.UserAddresses.GetDefaultAddressAsync(userId);

        if (currentDefault is not null)
        {
            currentDefault.IsDefault = false;
        }
    }
}