namespace Application.Jobs;

public interface ISupportChatCleanupJob
{
    Task DeleteExpiredClosedConversationsAsync();}