using Application.DTOs.SupportChat;
using Application.Interfaces;
using dotnet_ecommerce_api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace dotnet_ecommerce_api.Services;

public sealed class SignalRSupportChatNotifier : ISupportChatNotifier
{
    private readonly IHubContext<SupportChatHub> _hubContext;
    private readonly ILogger<SignalRSupportChatNotifier> _logger;

    public SignalRSupportChatNotifier(IHubContext<SupportChatHub> hubContext, ILogger<SignalRSupportChatNotifier> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyMessageSentAsync(int conversationId, SupportMessageDto message)
    {
        try
        {
            var groupName = SupportChatHub.GetConversationGroup(conversationId);

            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to broadcast message {MessageId} for conversation {ConversationId}.",
                message.SupportMessageId,
                conversationId);
        }
    }
}