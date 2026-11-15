using Application.DTOs.Common;
using Application.DTOs.SupportChat;

namespace Application.Interfaces.SupportChat
{
    public interface ISupportChatService
    {
        Task<SupportConversationDto> GetOrCreateConversationAsync(int userId);
        Task<SupportMessageDto> SendMessageAsync(int conversationId, int senderId, SendSupportMessageDto dto);
        Task<List<SupportMessageDto>> GetConversationMessagesAsync(int conversationId, int requesterId);
        Task<SupportConversationDto?> GetMyConversationAsync(int userId);
        Task<PagedResult<SupportConversationDto>> GetAllConversationsAsync(PaginationRequest request, CancellationToken cancellationToken = default);
        Task AssignConversationAsync(int conversationId, AssignSupportConversationDto dto);
        Task UpdateConversationStatusAsync(int conversationId, UpdateSupportConversationStatusDto dto);
        Task<int> DeleteExpiredClosedConversationsAsync(DateTime cutoffDate);
    }
}