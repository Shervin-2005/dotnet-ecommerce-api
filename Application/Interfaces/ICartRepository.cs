using Domain.Entities;

namespace Application.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(int userId);
}