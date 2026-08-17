using Application.DTOs.Auth;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, UpdateUserDto dto);
        Task<bool> SoftDeleteAsync(int userId);
    }
}
