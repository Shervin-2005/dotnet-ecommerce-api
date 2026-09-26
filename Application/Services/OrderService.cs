using Application.DTOs.Order;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class OrderService : IOrderService
{
    // Which statuses an order is allowed to move into from its current one.
    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
    {
        [OrderStatus.Pending] = new[] { OrderStatus.Paid, OrderStatus.Cancelled },
        [OrderStatus.Paid] = new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
        [OrderStatus.Shipped] = new[] { OrderStatus.Delivered },
        [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
        [OrderStatus.Cancelled] = Array.Empty<OrderStatus>()
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper,  ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderDto> CheckoutAsync(int userId, CreateOrderDto dto)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
        if (cart is null || cart.Items.Count == 0)
            throw new BadRequestException("Your cart is empty.");
        
        foreach (var item in cart.Items)
        {
            if (item.Quantity > item.Product.StockQuantity)
                throw new BadRequestException(
                    $"'{item.Product.ProductName}' only has {item.Product.StockQuantity} left in stock.");
        }

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Pending,
            ShippingAddress = dto.ShippingAddress
        };

        decimal total = 0;

        foreach (var item in cart.Items)
        {
            total += item.Product.Price * item.Quantity;

            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.Product.ProductName, 
                UnitPrice = item.Product.Price,           
                Quantity = item.Quantity
            });

            item.Product.StockQuantity -= item.Quantity;
            item.Product.SoldQuantity += item.Quantity;
        }

        order.TotalAmount = total;

        await _unitOfWork.Orders.AddAsync(order);

        foreach (var item in cart.Items.ToList())
            _unitOfWork.CartItems.Delete(item);
        
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId)
    {
        var orders = await _unitOfWork.Orders.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _unitOfWork.Orders.GetAllWithDetailsAsync();
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<(OrderActionResult Result, OrderDto? Order)> GetOrderDetailAsync(int orderId, int userId, bool isAdmin)
    {
        var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId);
        if (order is null) return (OrderActionResult.NotFound, null);
        if (!isAdmin && order.UserId != userId) return (OrderActionResult.Forbidden, null);

        return (OrderActionResult.Success, _mapper.Map<OrderDto>(order));
    }

    public async Task<OrderActionResult> UpdateStatusAsync(int orderId, UpdateOrderStatusDto dto)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order is null) return OrderActionResult.NotFound;

        if (!AllowedTransitions[order.Status].Contains(dto.Status))
            return OrderActionResult.InvalidStatusTransition;

        order.Status = dto.Status;
        order.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Orders.Update(order);
        
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation(
            "Order {OrderId} status changed from {OldStatus} to {NewStatus}.",
            orderId,
            order.Status,
            dto.Status);
        
        return OrderActionResult.Success;
    }

    public async Task<OrderActionResult> CancelAsync(int orderId, int userId)
    {
        var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(orderId);
        if (order is null) return OrderActionResult.NotFound;
        if (order.UserId != userId) return OrderActionResult.Forbidden;

        if (order.Status != OrderStatus.Pending)
            return OrderActionResult.InvalidStatusTransition;
        
        foreach (var item in order.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product is not null)
            {
                product.StockQuantity += item.Quantity;
                product.SoldQuantity -= item.Quantity;
            }
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation(
            "Order {OrderId} was cancelled by user {UserId}.",
            orderId,
            userId);
        
        return OrderActionResult.Success;
    }
}