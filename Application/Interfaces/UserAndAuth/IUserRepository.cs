using Domain.Entities;

namespace Application.Interfaces.UserAndAuth
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<User>> GetAllUsersWithoutFilter();
    }
}
