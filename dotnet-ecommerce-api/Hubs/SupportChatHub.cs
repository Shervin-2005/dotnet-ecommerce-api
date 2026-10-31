using System.Security.Claims;
using Application.Interfaces;
using Application.Interfaces.Common;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace dotnet_ecommerce_api.Hubs;

[Authorize]
public class SupportChatHub : Hub
{
    private readonly IUnitOfWork _unitOfWork;

    public SupportChatHub(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task JoinConversation(int conversationId)
    {
        var userId = GetAuthenticatedUserId();

        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user is null || user.IsDeleted)
            throw new HubException("User not found.");

        var conversation = await _unitOfWork.SupportConversations.GetByIdAsync(conversationId);

        if (conversation is null)
            throw new HubException("Conversation not found.");

        var isCustomer = user.Role == UserRole.Customer && conversation.UserId == userId;

        var isAssignedAdmin = user.Role == UserRole.Admin && conversation.AssignedAdminId == userId;

        if (!isCustomer && !isAssignedAdmin)
            throw new HubException("You are not authorized to join this conversation.");

        await Groups.AddToGroupAsync(Context.ConnectionId, GetConversationGroup(conversationId));
    }

    public async Task LeaveConversation(int conversationId)
    {
        var userId = GetAuthenticatedUserId();

        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user is null || user.IsDeleted)
            throw new HubException("User not found.");

        var conversation = await _unitOfWork.SupportConversations.GetByIdAsync(conversationId);

        if (conversation is null)
            throw new HubException("Conversation not found.");

        var isCustomer = user.Role == UserRole.Customer && conversation.UserId == userId;
        
        var isAssignedAdmin = user.Role == UserRole.Admin && conversation.AssignedAdminId == userId;

        if (!isCustomer && !isAssignedAdmin)
            throw new HubException("You are not authorized to leave this conversation.");

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetConversationGroup(conversationId));
    }

    private int GetAuthenticatedUserId()
    {
        var userIdValue =
            Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Context.User?.FindFirstValue("sub");

        if (!int.TryParse(userIdValue, out var userId))
            throw new HubException("Invalid authenticated user.");

        return userId;
    }

    public static string GetConversationGroup(int conversationId)
        => $"support-conversation-{conversationId}";
}