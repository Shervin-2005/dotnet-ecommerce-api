using Domain.Entities;

namespace Application.Interfaces.UserAndAuth;

public interface IUserAddressRepository : IGenericRepository<UserAddress>
{
    Task<UserAddress?> GetByIdAndUserIdAsync(int userAddressId, int userId);

    Task<UserAddress?> GetDefaultAddressAsync(int userId);

    Task<List<UserAddress>> GetUserAddressesAsync(int userId);
    
    Task<UserAddress?> GetNextDefaultCandidateAsync(int userId, int excludedAddressId);
}