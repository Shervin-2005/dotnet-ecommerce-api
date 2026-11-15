using Application.DTOs.Common;
using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.UserAndAuth; 
    public interface IUserRepository : IGenericRepository<User> 
    { 
        Task<User?> GetByPhoneNumberAsync(string phoneNumber); 
        Task<PagedResult<User>> GetAllActiveUsersPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<User>> GetAllDeletedUsersPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<PagedResult<User>> GetAllUsersPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task<User?> GetDeletedUserByIdAsync(int userId); 
    }