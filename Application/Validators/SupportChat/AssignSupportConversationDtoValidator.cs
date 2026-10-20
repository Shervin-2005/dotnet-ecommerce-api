using Application.DTOs.SupportChat;
using FluentValidation;

namespace Application.Validators.SupportChat;

public class AssignSupportConversationDtoValidator : AbstractValidator<AssignSupportConversationDto>
{
    public AssignSupportConversationDtoValidator()
    {
        RuleFor(x => x.AdminId)
            .GreaterThan(0)
            .WithMessage("Admin ID must be greater than zero.");
    }
}