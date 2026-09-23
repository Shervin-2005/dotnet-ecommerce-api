using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IMapper _mapper;
    private ILogger<PaymentService>_logger;

    public PaymentService(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway, IMapper mapper, ILogger <PaymentService> logger) 
    {
        _unitOfWork = unitOfWork;
        _paymentGateway = paymentGateway;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<(PaymentActionResult Result, PaymentDto? Payment)> PayOrderAsync(int orderId, int userId, PayOrderDto dto)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order is null) return (PaymentActionResult.OrderNotFound, null);
        if (order.UserId != userId)
        {
            _logger.LogWarning( "Unauthorized payment attempt for OrderId {OrderId} by UserId {UserId}", orderId, userId);
            
            return (PaymentActionResult.Forbidden, null);
        }
        
        if (order.Status != OrderStatus.Pending)
            return (PaymentActionResult.OrderNotPayable, null);
        
        _logger.LogInformation( "Payment processing started for OrderId {OrderId}," +
                                " UserId {UserId}," +
                                " Amount {Amount}",
            orderId, userId, order.TotalAmount);

        var digitsOnly = new string(dto.CardNumber.Where(char.IsDigit).ToArray());
        var last4 = digitsOnly.Length >= 4 ? digitsOnly[^4..] : null;

        var result = await _paymentGateway.ChargeAsync(new PaymentRequest(
            order.TotalAmount,
            dto.CardNumber,
            dto.CardHolderName,
            dto.ExpiryMonth,
            dto.ExpiryYear,
            dto.Cvv,
            order.OrderId));
        
        var payment = new Payment
        {
            OrderId = order.OrderId,
            Amount = order.TotalAmount,
            CardLast4 = last4,
            Status = result.IsSuccess ? PaymentStatus.Succeeded : PaymentStatus.Failed,
            TransactionId = result.TransactionId,
            FailureReason = result.FailureReason,
            ProcessedAt = DateTime.UtcNow
        };

        await _unitOfWork.Payments.AddAsync(payment);

        if (result.IsSuccess)
        {
            order.Status = OrderStatus.Paid;
            order.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Orders.Update(order);
            
            _logger.LogInformation( "Payment succeeded for OrderId {OrderId}," +
                                    " TransactionId {TransactionId}",
                orderId, result.TransactionId);
        }
        else
        {
            _logger.LogWarning( "Payment declined for OrderId {OrderId}," +
                                " TransactionId {TransactionId}," +
                                " FailureReason {FailureReason}",
                orderId, result.TransactionId, result.FailureReason);
        }
        await _unitOfWork.SaveChangesAsync();

        return result.IsSuccess
            ? (PaymentActionResult.Success, _mapper.Map<PaymentDto>(payment))
            : (PaymentActionResult.PaymentDeclined, _mapper.Map<PaymentDto>(payment));
    }

    public async Task<(PaymentActionResult Result, IEnumerable<PaymentDto>? Payments)> GetOrderPaymentsAsync(int orderId, int userId, bool isAdmin)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order is null) return (PaymentActionResult.OrderNotFound, null);
        if (!isAdmin && order.UserId != userId)
        {
            _logger.LogWarning( "Unauthorized access attempt to payments for OrderId {OrderId} " +
                                "by UserId {UserId}",
                orderId, userId);
            
            return (PaymentActionResult.Forbidden, null);
        }

        var payments = await _unitOfWork.Payments.GetByOrderIdAsync(orderId);
        return (PaymentActionResult.Success, _mapper.Map<IEnumerable<PaymentDto>>(payments));
    }
}