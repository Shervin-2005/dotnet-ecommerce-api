using Domain.Entities;

namespace Application.Interfaces.Payment;

public interface IPaymentRepository : IGenericRepository<Domain.Entities.Payment>
{
    Task<List<Domain.Entities.Payment>> GetByOrderIdAsync(int orderId);
}