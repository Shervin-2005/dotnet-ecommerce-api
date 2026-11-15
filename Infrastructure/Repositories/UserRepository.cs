using Application.DTOs.Common;
using Application.Interfaces.UserAndAuth;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {

        }
        
        public async Task<PagedResult<User>> GetAllActiveUsersPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking().Where(u => !u.IsDeleted);

            return await GetPagedUsersAsync(query, request, cancellationToken);
        }
        
        public async Task<PagedResult<User>> GetAllDeletedUsersPagedAsync(
            PaginationRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.IgnoreQueryFilters().AsNoTracking().Where(u => u.IsDeleted);

            return await GetPagedUsersAsync(query, request, cancellationToken);
        }
        
        public async Task<PagedResult<User>> GetAllUsersPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.IgnoreQueryFilters().AsNoTracking();

            return await GetPagedUsersAsync(query, request, cancellationToken);
        }
        
        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber) =>
            await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        
        public async Task<User?> GetDeletedUserByIdAsync(int userId) =>
            await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.UserId == userId && u.IsDeleted);
        
        private async Task<PagedResult<User>> GetPagedUsersAsync(IQueryable<User> query, PaginationRequest request, CancellationToken cancellationToken)
        {
            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderBy(u => u.UserId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<User>
            {
                Items = users,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
