using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(100)
            .WithMessage("First name cannot be longer than 100 characters.")
            .When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .WithMessage("Last name cannot be longer than 100 characters.")
            .When(x => x.LastName is not null);
    }
}