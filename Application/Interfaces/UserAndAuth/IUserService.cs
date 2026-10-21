using Application.DTOs.UserAndAuth;
using Domain.Enums;

namespace Application.Interfaces.UserAndAuth
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, UpdateUserDto dto);
        Task<bool> SoftDeleteAsync(int userId);
        Task<IEnumerable<UserDto>> GetAllActiveUsersAsync();
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
    }
}
