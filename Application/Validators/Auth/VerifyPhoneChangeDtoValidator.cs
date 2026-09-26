using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class VerifyPhoneChangeDtoValidator : AbstractValidator<VerifyPhoneChangeDto>
{
    public VerifyPhoneChangeDtoValidator()
    {
        RuleFor(x => x.NewPhoneNumber)
            .NotEmpty()
            .WithMessage("New phone number is required.")
            .Matches(@"^(?:\+98|0098|98|0)?9\d{9}$")
            .WithMessage("Please enter a valid new phone number.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Verification code is required.")
            .Length(6)
            .WithMessage("Verification code must be 6 digits long.")
            .Matches(@"^\d{6}$")
            .WithMessage("Verification code must contain only digits.");
    }
}