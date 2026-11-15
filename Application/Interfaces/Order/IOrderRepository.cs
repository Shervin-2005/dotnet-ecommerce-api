using Application.DTOs.Common;
using Application.Interfaces.Common;
using Domain.Entities;

namespace Application.Interfaces.Order;

public interface IOrderRepository : IGenericRepository<Domain.Entities.Order>
{
    Task<Domain.Entities.Order?> GetByIdWithDetailsAsync(int orderId);
    Task<PagedResult<Domain.Entities.Order>> GetByUserIdPagedWithDetailsAsync(int userId, PaginationRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<Domain.Entities.Order>> GetAllPagedWithDetailsAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<List<Domain.Entities.Order>> GetPendingOrdersOlderThanAsync(DateTime expirationTime);
}
