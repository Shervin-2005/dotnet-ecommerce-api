namespace Application.Jobs;

public interface IOrderCleanupJob
{
    Task CancelExpiredPendingOrdersAsync();
}