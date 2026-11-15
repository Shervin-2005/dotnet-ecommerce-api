using Application.DTOs.Common;
using Application.DTOs.UserAndAuth;
using Domain.Enums;

namespace Application.Interfaces.UserAndAuth
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, UpdateUserDto dto);
        Task<bool> SoftDeleteAsync(int userId);
        Task<PagedResult<UserDto>> GetAllDeletedUsersAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<bool> RestoreUserAsync(int userId);
        Task<PagedResult<UserDto>> GetAllActiveUsersAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<UserDto>> GetAllUsersAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    }
}
