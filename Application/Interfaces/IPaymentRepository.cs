using Domain.Entities;

namespace Application.Interfaces;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<List<Payment>> GetByOrderIdAsync(int orderId);
}