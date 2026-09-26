using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class VerifyRegistrationOtpDtoValidator : AbstractValidator<VerifyRegistrationOtpDto>
{
    public VerifyRegistrationOtpDtoValidator()
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
            .WithMessage("Verification code must be 6 digits long.")
            .Matches(@"^\d{6}$")
            .WithMessage("Verification code must contain only digits.");
    }
}