using Application.DTOs.SupportChat;
using FluentValidation;

namespace Application.Validators.SupportChat;

public class SendSupportMessageDtoValidator : AbstractValidator<SendSupportMessageDto>
{
    public SendSupportMessageDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(4000)
            .Must(content => !string.IsNullOrWhiteSpace(content))
            .WithMessage("Message content cannot be empty or whitespace.");
    }
}