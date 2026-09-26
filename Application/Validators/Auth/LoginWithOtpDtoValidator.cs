using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class LoginWithOtpDtoValidator : AbstractValidator<LoginWithOtpDto>
{
    public LoginWithOtpDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^(?:\+98|0098|98|0)?9\d{9}$")
            .WithMessage("Please enter a valid phone number.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Verification code is required.")
            .Length(6)
            .WithMessage("Verification code must be 6 characters.")
            .Matches(@"^\d{6}$")
            .WithMessage("Verification code must contain only digits.");
    }
}