namespace Application.Interfaces.Cart;

public interface ICartRepository : IGenericRepository<Domain.Entities.Cart>
{
    Task<Domain.Entities.Cart?> GetByUserIdAsync(int userId);
}