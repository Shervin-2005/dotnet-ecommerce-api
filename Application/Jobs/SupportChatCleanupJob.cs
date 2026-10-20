using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Jobs;

public sealed class SupportChatCleanupJob : ISupportChatCleanupJob
{
    private readonly ISupportChatService _supportChatService;
    private readonly ILogger<SupportChatCleanupJob> _logger;

    public SupportChatCleanupJob(
        ISupportChatService supportChatService,
        ILogger<SupportChatCleanupJob> logger)
    {
        _supportChatService = supportChatService;
        _logger = logger;
    }

    public async Task DeleteExpiredClosedConversationsAsync()
    {
        _logger.LogInformation(
            "Starting closed support conversations cleanup.");

        var cutoffDate = DateTime.UtcNow.AddDays(-90);

        var deletedCount =
            await _supportChatService
                .DeleteExpiredClosedConversationsAsync(cutoffDate);

        _logger.LogInformation(
            "Closed support conversations cleanup completed. " +
            "Deleted {DeletedCount} conversations.",
            deletedCount);
    }
}