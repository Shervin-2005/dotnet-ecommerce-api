using Application.Interfaces;
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
        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber) =>
            await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

        public async Task<IEnumerable<User>> GetAllUsersWithoutFilter() =>
            await _dbSet.IgnoreQueryFilters().ToListAsync();
    }
}
