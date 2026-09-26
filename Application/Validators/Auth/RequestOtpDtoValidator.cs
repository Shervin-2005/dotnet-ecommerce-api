using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class RequestOtpDtoValidator : AbstractValidator<RequestOtpDto>
{
    public RequestOtpDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^(?:\+98|0098|98|0)?9\d{9}$")
            .WithMessage("Please enter a valid phone number.");
    }
}