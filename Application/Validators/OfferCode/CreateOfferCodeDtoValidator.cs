using Application.DTOs.OfferCode;
using FluentValidation;

namespace Application.Validators.OfferCode;

public class CreateOfferCodeDtoValidator
    : AbstractValidator<CreateOfferCodeDto>
{
    public CreateOfferCodeDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[A-Za-z0-9_-]+$")
            .WithMessage(
                "Offer code can only contain letters, numbers, underscores, and hyphens.");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(1, 100)
            .WithMessage(
                "Discount percentage must be between 1 and 100.");

        RuleFor(x => x.MinimumOrderAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinimumOrderAmount.HasValue)
            .WithMessage(
                "Minimum order amount cannot be negative.");

        RuleFor(x => x.MaximumDiscountAmount)
            .GreaterThan(0)
            .When(x => x.MaximumDiscountAmount.HasValue)
            .WithMessage(
                "Maximum discount amount must be greater than zero.");

        RuleFor(x => x.UsageLimit)
            .GreaterThan(0)
            .When(x => x.UsageLimit.HasValue)
            .WithMessage(
                "Usage limit must be greater than zero.");

        RuleFor(x => x.UserUsageLimit)
            .GreaterThan(0)
            .When(x => x.UserUsageLimit.HasValue)
            .WithMessage(
                "User usage limit must be greater than zero.");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(x => x.StartsAt)
            .WithMessage(
                "Expiration date must be after the start date.");
    }
}