using Application.DTOs.SupportChat;
using FluentValidation;

namespace Application.Validators.SupportChat;

public class UpdateSupportConversationStatusDtoValidator : AbstractValidator<UpdateSupportConversationStatusDto>
{
    public UpdateSupportConversationStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid conversation status.");
    }
}