using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class AddPasswordDtoValidator : AbstractValidator<AddPasswordDto>
{
    public AddPasswordDtoValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password length at least 8 characters")
            .MaximumLength(64)
            .WithMessage("Password length must be less than 64 characters");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword);

        RuleFor(x => x.Otp)
            .NotEmpty();
    }
}