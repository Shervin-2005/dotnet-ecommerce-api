using Application.DTOs.SupportChat;

namespace Application.Interfaces
{
    public interface ISupportChatService
    {
        Task<SupportConversationDto> GetOrCreateConversationAsync(int userId);

        Task<SupportMessageDto> SendMessageAsync(int conversationId, int senderId, SendSupportMessageDto dto);

        Task<List<SupportMessageDto>> GetConversationMessagesAsync(int conversationId, int requesterId);

        Task<SupportConversationDto?> GetMyConversationAsync(int userId);

        Task<List<SupportConversationDto>> GetAllConversationsAsync();

        Task AssignConversationAsync(int conversationId, AssignSupportConversationDto dto);

        Task UpdateConversationStatusAsync(int conversationId, UpdateSupportConversationStatusDto dto);
    }
}