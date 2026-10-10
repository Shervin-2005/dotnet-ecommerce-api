using Application.DTOs.OfferCode;
using Application.DTOs.Order;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
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
    private readonly IOfferCodeService _offerCodeService;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper,  ILogger<OrderService> logger, IOfferCodeService offerCodeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _offerCodeService = offerCodeService;
    }

   public async Task<OrderDto> CheckoutAsync(
    int userId,
    CreateOrderDto dto)
{
    var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);

    if (cart is null || cart.Items.Count == 0)
        throw new BadRequestException("Your cart is empty.");

    var address =
        await _unitOfWork.UserAddresses
            .GetByIdAndUserIdAsync(
                dto.UserAddressId,
                userId);

    if (address is null)
        throw new NotFoundException("Address not found.");

    decimal subtotal = 0;

    foreach (var item in cart.Items)
    {
        var availableStock =
            item.ProductVariant?.StockQuantity
            ?? item.Product.StockQuantity;

        if (item.Quantity > availableStock)
        {
            var productName = item.Product.ProductName;

            throw new BadRequestException(
                $"'{productName}' only has {availableStock} left in stock.");
        }

        var unitPrice =
            item.ProductVariant?.SalePrice
            ?? item.Product.SalePrice;

        subtotal += unitPrice * item.Quantity;
    }

    OfferCodeCalculationResult? offerResult = null;

    if (!string.IsNullOrWhiteSpace(dto.OfferCode))
    {
        offerResult =
            await _offerCodeService.CalculateDiscountAsync(
                dto.OfferCode,
                userId,
                subtotal);
    }

    var discountAmount =
        offerResult?.DiscountAmount ?? 0;

    var totalAmount =
        subtotal - discountAmount;

    var order = new Order
    {
        UserId = userId,
        Status = OrderStatus.Pending,

        SubtotalAmount = subtotal,
        DiscountAmount = discountAmount,
        TotalAmount = totalAmount,

        RecipientName = address.RecipientName,
        PhoneNumber = address.PhoneNumber,
        Province = address.Province,
        City = address.City,
        AddressLine = address.AddressLine,
        PostalCode = address.PostalCode
    };

    foreach (var item in cart.Items)
    {
        var unitPrice =
            item.ProductVariant?.SalePrice
            ?? item.Product.SalePrice;

        order.Items.Add(new OrderItem
        {
            ProductId = item.ProductId,
            ProductVariantId = item.ProductVariantId,

            ProductName = item.Product.ProductName,
            VariantSku = item.ProductVariant?.Sku,

            UnitPrice = unitPrice,
            Quantity = item.Quantity
        });

        if (item.ProductVariant is not null)
        {
            item.ProductVariant.StockQuantity -= item.Quantity;
            item.ProductVariant.SoldQuantity += item.Quantity;
        }
        else
        {
            item.Product.StockQuantity -= item.Quantity;
            item.Product.SoldQuantity += item.Quantity;
        }
    }

    if (offerResult is not null)
    {
        order.OfferCodeUsage = new OfferCodeUsage
        {
            OfferCodeId = offerResult.OfferCodeId,
            UserId = userId,
            CodeSnapshot = offerResult.Code,
            DiscountAmount = offerResult.DiscountAmount,
            UsedAt = DateTime.UtcNow
        };

        var offerCode =
            await _unitOfWork.OfferCodes
                .GetByIdAsync(offerResult.OfferCodeId);

        if (offerCode is null)
            throw new NotFoundException("Offer code not found.");

        offerCode.UsedCount++;
    }

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

        var oldStatus = order.Status;
        
        order.Status = dto.Status;
        order.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Orders.Update(order);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(
                ex,
                "Concurrency conflict while updating Order {OrderId}.",
                orderId);

            return OrderActionResult.InvalidStatusTransition;
        }
        
        _logger.LogInformation(
            "Order {OrderId} status changed from {OldStatus} to {NewStatus}.",
            orderId,
            oldStatus,
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
        
        try
        { 
            foreach (var item in order.Items)
            {
                if (item.ProductVariant is not null)
                {
                    item.ProductVariant.StockQuantity += item.Quantity;
                    item.ProductVariant.SoldQuantity -= item.Quantity;
                }
                else
                {
                    item.Product.StockQuantity += item.Quantity;
                    item.Product.SoldQuantity -= item.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

             _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();
            
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(
                ex,
                "Concurrency conflict while cancelling Order {OrderId} by UserId {UserId}.",
                orderId,
                userId);

            return OrderActionResult.InvalidStatusTransition;
        }
        _logger.LogInformation(
            "Order {OrderId} was cancelled by user {UserId}.",
            orderId,
            userId);
        
        return OrderActionResult.Success;
    }

    public async Task CancelExpiredPendingOrdersAsync()
    {
        try
        {
            var expirationTime = DateTime.UtcNow.AddMinutes(-30);
            
            var orders = await _unitOfWork.Orders
                .GetPendingOrdersOlderThanAsync(expirationTime);

            foreach (var order in orders)
            {
                if (order.Status != OrderStatus.Pending) continue;
            
                foreach (var item in order.Items)
                {
                    if (item.ProductVariant is not null)
                    {
                        item.ProductVariant.StockQuantity += item.Quantity;
                        item.ProductVariant.SoldQuantity -= item.Quantity;
                    }
                    else
                    {
                        item.Product.StockQuantity += item.Quantity;
                        item.Product.SoldQuantity -= item.Quantity;
                    }
                }
                order.Status = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;
                order.Version = Guid.NewGuid();
            }

       
            await _unitOfWork.SaveChangesAsync();
            
        }
        catch (DbUpdateConcurrencyException ex)
        {
                _logger.LogWarning(
                    ex, "Concurrency conflict while cancelling expired orders.");
        }
    }
}