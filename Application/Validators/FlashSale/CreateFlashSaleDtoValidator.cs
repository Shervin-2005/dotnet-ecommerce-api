using Application.DTOs.FlashSale;
using FluentValidation;

namespace Application.Validators.FlashSale;

public class CreateFlashSaleDtoValidator : AbstractValidator<CreateFlashSaleDto>
{
    public CreateFlashSaleDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Campaign title is required.")
            .MaximumLength(150)
            .WithMessage("Campaign title cannot exceed 150 characters.");

        RuleFor(x => x.StartsAt)
            .Must(x => x.Kind == DateTimeKind.Utc)
            .WithMessage("Start date must be in UTC.");

        RuleFor(x => x.EndsAt)
            .Must(x => x.Kind == DateTimeKind.Utc)
            .WithMessage("End date must be in UTC.");

        RuleFor(x => x.EndsAt)
            .GreaterThan(x => x.StartsAt)
            .WithMessage("End date must be later than start date.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order cannot be negative.");
    }
}