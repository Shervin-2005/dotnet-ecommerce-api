using Application.DTOs;
using Application.DTOs.UserAndAuth;

namespace Application.Interfaces;

public interface IUserAddressService
{
    Task<List<UserAddressDto>> GetUserAddressesAsync(int userId);

    Task<UserAddressDto> GetByIdAsync(int userId, int userAddressId);

    Task<UserAddressDto> CreateAsync(int userId, CreateUserAddressDto dto);

    Task<UserAddressDto> UpdateAsync(int userId, int userAddressId, UpdateUserAddressDto dto);

    Task DeleteAsync(int userId, int userAddressId);
}