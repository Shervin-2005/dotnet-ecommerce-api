using Application.DTOs.SupportChat;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services
{
    public class SupportChatService : ISupportChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISupportChatNotifier _supportChatNotifier;

        public SupportChatService(IUnitOfWork unitOfWork, ISupportChatNotifier supportChatNotifier)
        {
            _unitOfWork = unitOfWork;
            _supportChatNotifier = supportChatNotifier;
        }
        
        public async Task<SupportConversationDto> GetOrCreateConversationAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user is null || user.IsDeleted)
                throw new NotFoundException("User not found.");

            if (user.Role != UserRole.Customer)
                throw new UnauthorizedException("Only customers can start support conversations.");

            var existing = await _unitOfWork.SupportConversations.GetActiveByUserIdAsync(userId);

            if (existing is not null)
                return MapConversation(existing);

            var conversation = new SupportConversation
            {
                UserId = userId,
                Status = SupportConversationStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.SupportConversations.AddAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            return MapConversation(conversation, user);
        }
        
        public async Task<SupportMessageDto> SendMessageAsync(int conversationId, int senderId, SendSupportMessageDto dto)
        {
            var sender = await _unitOfWork.Users.GetByIdAsync(senderId);

            if (sender is null || sender.IsDeleted)
                throw new NotFoundException("Sender not found.");

            var conversation = await _unitOfWork.SupportConversations.GetWithUsersAsync(conversationId);

            if (conversation is null)
                throw new NotFoundException("Support conversation not found.");

            if (conversation.Status == SupportConversationStatus.Closed)
                throw new BadRequestException("Cannot send messages to a closed conversation.");

            if (sender.Role == UserRole.Customer)
            {
                if (conversation.UserId != senderId) throw new UnauthorizedException("You cannot send messages to another customer's conversation.");
            }
            else if (sender.Role == UserRole.Admin)
            {
                if (conversation.AssignedAdminId != senderId)
                    throw new UnauthorizedException("You can only reply to conversations assigned to you.");
            }
            else
            {
                throw new UnauthorizedException("You are not allowed to send support messages.");
            }

            var message = new SupportMessage
            {
                SupportConversationId = conversationId,
                SenderId = senderId,
                Content = dto.Content.Trim(),
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _unitOfWork.SupportMessages.AddAsync(message);
            
            if (sender.Role == UserRole.Admin)
            {
                conversation.Status = SupportConversationStatus.InProgress;
                _unitOfWork.SupportConversations.Update(conversation);
            }

            await _unitOfWork.SaveChangesAsync();
            
            var messageDto = MapMessage(message, sender);

            await _supportChatNotifier.NotifyMessageSentAsync(conversationId, messageDto);

            return messageDto;
        }
        
        public async Task<List<SupportMessageDto>> GetConversationMessagesAsync(int conversationId, int requesterId)
        { 
            var requester = await _unitOfWork.Users.GetByIdAsync(requesterId);

            if (requester is null || requester.IsDeleted)
                throw new NotFoundException("User not found.");

            var conversation = await _unitOfWork.SupportConversations.GetByIdAsync(conversationId);

            if (conversation is null)
                throw new NotFoundException("Support conversation not found.");

            if (requester.Role == UserRole.Customer && conversation.UserId != requesterId)
                throw new UnauthorizedException("You cannot access another customer's conversation.");
            

            if (requester.Role != UserRole.Customer && requester.Role != UserRole.Admin)
                throw new UnauthorizedException("You are not allowed to access support messages.");
            
            var messages = await _unitOfWork.SupportMessages.GetConversationMessagesAsync(conversationId);

            return messages
                .Select(message => MapMessage(message, message.Sender))
                .ToList();
        }
        
        public async Task<SupportConversationDto?> GetMyConversationAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user is null || user.IsDeleted) throw new NotFoundException("User not found.");

            if (user.Role != UserRole.Customer)
                throw new UnauthorizedException("This operation is available to customers only.");

            var active = await _unitOfWork.SupportConversations.GetActiveByUserIdAsync(userId);

            if (active is null)
                return null;

            var conversation = await _unitOfWork.SupportConversations.GetWithUsersAsync(active.SupportConversationId);

            return conversation is null ? null : MapConversation(conversation);
        }
        
        public async Task<List<SupportConversationDto>> GetAllConversationsAsync()
        {
            var conversations = await _unitOfWork.SupportConversations.GetAllWithUsersAsync();

            return conversations
                .Select(conversation => MapConversation(conversation))
                .ToList();
        }
        
        public async Task AssignConversationAsync(int conversationId, AssignSupportConversationDto dto)
        {
            var conversation = await _unitOfWork.SupportConversations.GetByIdAsync(conversationId);

            if (conversation is null)
                throw new NotFoundException("Support conversation not found.");

            if (conversation.Status == SupportConversationStatus.Closed)
                throw new BadRequestException("A closed conversation cannot be assigned.");

            var targetAdmin = await _unitOfWork.Users.GetByIdAsync(dto.AdminId);

            if (targetAdmin is null || targetAdmin.IsDeleted || targetAdmin.Role != UserRole.Admin)
                throw new BadRequestException("The selected user is not an active administrator.");

            conversation.AssignedAdminId = targetAdmin.UserId;
            conversation.Status = SupportConversationStatus.InProgress;

            _unitOfWork.SupportConversations.Update(conversation);

            await _unitOfWork.SaveChangesAsync();
        }
        
        public async Task UpdateConversationStatusAsync(int conversationId, UpdateSupportConversationStatusDto dto)
        {
            var conversation = await _unitOfWork.SupportConversations.GetByIdAsync(conversationId);

            if (conversation is null)
                throw new NotFoundException("Support conversation not found.");

            if (conversation.Status == SupportConversationStatus.Closed)
                throw new BadRequestException("A closed conversation cannot be reopened.");

            if (dto.Status == SupportConversationStatus.Open)
                throw new BadRequestException("An administrator cannot reopen a conversation.");

            if (dto.Status == SupportConversationStatus.InProgress && conversation.AssignedAdminId is null)
                throw new BadRequestException("Assign the conversation before marking it InProgress.");
            
            conversation.Status = dto.Status;

            conversation.ClosedAt = dto.Status == SupportConversationStatus.Closed ? DateTime.UtcNow : null;

            _unitOfWork.SupportConversations.Update(conversation);

            await _unitOfWork.SaveChangesAsync();
        }

        private static SupportConversationDto MapConversation(SupportConversation conversation, User? user = null)
        {
            user ??= conversation.User;

            return new SupportConversationDto
            {
                SupportConversationId = conversation.SupportConversationId,

                UserId = conversation.UserId,

                UserName = GetDisplayName(user),

                AssignedAdminId = conversation.AssignedAdminId,

                AssignedAdminName = conversation.AssignedAdmin is null ? null : GetDisplayName(conversation.AssignedAdmin),

                Status = conversation.Status,

                CreatedAt = conversation.CreatedAt,

                ClosedAt = conversation.ClosedAt
            };
        }

        private static SupportMessageDto MapMessage(SupportMessage message, User sender)
        {
            return new SupportMessageDto
            {
                SupportMessageId = message.SupportMessageId,

                SupportConversationId = message.SupportConversationId,

                SenderId = message.SenderId,

                SenderName = GetDisplayName(sender),

                Content = message.Content,

                SentAt = message.SentAt,

                IsRead = message.IsRead
            };
        }
        
        public async Task<int> DeleteExpiredClosedConversationsAsync(DateTime cutoffDate)
        {
            var deletedCount = await _unitOfWork.SupportConversations.DeleteExpiredClosedAsync(cutoffDate);

            return deletedCount;
        }

        private static string GetDisplayName(User user)
        {
            var fullName = $"{user.FirstName} {user.LastName}".Trim();

            return string.IsNullOrWhiteSpace(fullName) ? user.PhoneNumber : fullName;
        }
    }
}