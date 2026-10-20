using System.Security.Claims;
using Application.DTOs.SupportChat;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller;

[ApiController]
[Route("api/support-chat")]
[Authorize]
public class SupportChatController : ControllerBase
{
    private readonly ISupportChatService _supportChatService;

    public SupportChatController(ISupportChatService supportChatService)
    {
        _supportChatService = supportChatService;
    }

    [HttpPost("conversation")]
    public async Task<IActionResult> GetOrCreateConversation()
    {
        var userId = GetAuthenticatedUserId();

        var conversation = await _supportChatService.GetOrCreateConversationAsync(userId);

        return Ok(conversation);
    }

    [HttpGet("conversation/me")]
    public async Task<IActionResult> GetMyConversation()
    {
        var userId = GetAuthenticatedUserId();

        var conversation = await _supportChatService.GetMyConversationAsync(userId);

        if (conversation is null)
            return NoContent();

        return Ok(conversation);
    }

    [HttpGet("conversation/{conversationId:int}/messages")]
    public async Task<IActionResult> GetConversationMessages(int conversationId)
    {
        var userId = GetAuthenticatedUserId();

        var messages = await _supportChatService.GetConversationMessagesAsync(conversationId, userId);

        return Ok(messages);
    }

    [HttpPost("conversation/{conversationId:int}/messages")]
    public async Task<IActionResult> SendMessage(int conversationId, [FromBody] SendSupportMessageDto dto)
    {
        var userId = GetAuthenticatedUserId();

        var message = await _supportChatService.SendMessageAsync(conversationId, userId, dto);

        return Ok(message);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("admin/conversations")]
    public async Task<IActionResult> GetAllConversations()
    {
        var conversations = await _supportChatService.GetAllConversationsAsync();

        return Ok(conversations);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("admin/conversations/{conversationId:int}/assign")]
    public async Task<IActionResult> AssignConversation(int conversationId, [FromBody] AssignSupportConversationDto dto)
    {
        await _supportChatService.AssignConversationAsync(conversationId, dto);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("admin/conversations/{conversationId:int}/status")]
    public async Task<IActionResult> UpdateConversationStatus(int conversationId,
        [FromBody] UpdateSupportConversationStatusDto dto)
    {
        await _supportChatService.UpdateConversationStatusAsync(conversationId, dto);

        return NoContent();
    }

    private int GetAuthenticatedUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
            throw new UnauthorizedAccessException("Invalid authenticated user.");

        return userId;
    }
}