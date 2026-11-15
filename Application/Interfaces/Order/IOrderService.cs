using Application.DTOs.Common;
using Application.DTOs.Order;
using Domain.Enums;

namespace Application.Interfaces.Order;

public interface IOrderService
{
    Task<OrderDto> CheckoutAsync(int userId, CreateOrderDto dto);
    Task<PagedResult<OrderDto>> GetMyOrdersAsync(int userId, PaginationRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<OrderDto>> GetAllOrdersAsync(PaginationRequest request, CancellationToken cancellationToken = default);
    Task<(OrderActionResult Result, OrderDto? Order)> GetOrderDetailAsync(int orderId, int userId, bool isAdmin);
    Task<OrderActionResult> UpdateStatusAsync(int orderId, UpdateOrderStatusDto dto);
    Task<OrderActionResult> CancelAsync(int orderId, int userId);
    Task CancelExpiredPendingOrdersAsync();
}