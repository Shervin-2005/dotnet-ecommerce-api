using Application.DTOs.Payment;
using Domain.Enums;

namespace Application.Interfaces;

public interface IPaymentService
{
    Task<(PaymentActionResult Result, PaymentDto? Payment)> PayOrderAsync(int orderId, int userId, PayOrderDto dto);
    Task<(PaymentActionResult Result, IEnumerable<PaymentDto>? Payments)> GetOrderPaymentsAsync(int orderId, int userId, bool isAdmin);
}