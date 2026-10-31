using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Order;

public interface IOrderRepository : IGenericRepository<Domain.Entities.Order>
{
    Task<Domain.Entities.Order?> GetByIdWithDetailsAsync(int orderId);
    Task<List<Domain.Entities.Order>> GetByUserIdAsync(int userId);
    Task<List<Domain.Entities.Order>> GetAllWithDetailsAsync();
    Task<List<Domain.Entities.Order>> GetPendingOrdersOlderThanAsync(DateTime expirationTime);
}
