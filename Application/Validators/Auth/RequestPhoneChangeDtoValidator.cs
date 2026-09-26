using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class RequestPhoneChangeDtoValidator : AbstractValidator<RequestPhoneChangeDto>
{
    public RequestPhoneChangeDtoValidator()
    {
        RuleFor(x => x.NewPhoneNumber)
            .NotEmpty()
            .WithMessage("New phone number is required.")
            .Matches(@"^(?:\+98|0098|98|0)?9\d{9}$")
            .WithMessage("Please enter a valid new phone number.");
    }
}