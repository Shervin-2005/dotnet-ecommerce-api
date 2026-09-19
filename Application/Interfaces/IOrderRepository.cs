using Domain.Entities;

namespace Application.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetByIdWithDetailsAsync(int orderId);
    Task<List<Order>> GetByUserIdAsync(int userId);
    Task<List<Order>> GetAllWithDetailsAsync();
}