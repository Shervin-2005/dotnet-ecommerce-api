using Application.DTOs.Auth;
using Domain.Enums;

namespace Application.Interfaces
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
