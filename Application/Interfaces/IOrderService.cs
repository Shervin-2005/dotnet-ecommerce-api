using Application.DTOs;
using Domain.Enums;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CheckoutAsync(int userId, CreateOrderDto dto);
    Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId);
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<(OrderActionResult Result, OrderDto? Order)> GetOrderDetailAsync(int orderId, int userId, bool isAdmin);
    Task<OrderActionResult> UpdateStatusAsync(int orderId, UpdateOrderStatusDto dto);
    Task<OrderActionResult> CancelAsync(int orderId, int userId);
}