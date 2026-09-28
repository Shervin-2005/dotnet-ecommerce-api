using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Jobs;

public class OrderCleanupJob : IOrderCleanupJob
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderCleanupJob> _logger;

    public OrderCleanupJob(
        IOrderService orderService,
        ILogger<OrderCleanupJob> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    public async Task CancelExpiredPendingOrdersAsync()
    {
        _logger.LogInformation(
            "Starting expired pending orders cleanup.");

        await _orderService.CancelExpiredPendingOrdersAsync();

        _logger.LogInformation(
            "Expired pending orders cleanup completed.");
    }
}