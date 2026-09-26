using FluentValidation;
using dotnet_ecommerce_api.Models;

namespace dotnet_ecommerce_api.Validators;

public class BrandRequestValidator : AbstractValidator<BrandRequest>
{
    public BrandRequestValidator()
    {
        RuleFor(x => x.BrandName)
            .NotEmpty()
            .WithMessage("Brand name is required.")
            .MaximumLength(100)
            .WithMessage("Brand name cannot be longer than 100 characters.");

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("Brand image is required.");
    }
}