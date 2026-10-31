using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.UserAndAuth
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<User>> GetAllUsersWithoutFilter();
        Task<IEnumerable<User>> GetAllDeletedUsersAsync();
        Task<User?> GetDeletedUserByIdAsync(int userId);
    }
}
