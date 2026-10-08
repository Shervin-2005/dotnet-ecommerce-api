using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserAddressRepository : GenericRepository<UserAddress>, IUserAddressRepository
{
    public UserAddressRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<UserAddress?> GetByIdAndUserIdAsync(int userAddressId, int userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x =>
                x.UserAddressId == userAddressId &&
                x.UserId == userId);
    }

    public async Task<UserAddress?> GetDefaultAddressAsync(int userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.IsDefault);
    }

    public async Task<List<UserAddress>> GetUserAddressesAsync(int userId)
    {
        return await _dbSet
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.UserAddressId)
            .ToListAsync();
    }

    public async Task<UserAddress?> GetNextDefaultCandidateAsync(int userId, int excludedAddressId)
    {
        return await _dbSet
            .Where(x =>
                x.UserId == userId &&
                x.UserAddressId != excludedAddressId)
            .OrderBy(x => x.UserAddressId)
            .FirstOrDefaultAsync();
    }
}