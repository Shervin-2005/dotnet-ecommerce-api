namespace Application.Jobs;

public interface IOtpCleanupJob
{
    Task DeleteExpiredOtpsAsync();
}