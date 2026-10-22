using FluentValidation;
using dotnet_ecommerce_api.Models;

namespace dotnet_ecommerce_api.Validators;

public class CreateBrandRequestValidator : AbstractValidator<CreateBrandRequest>
{
    public CreateBrandRequestValidator()
    {
        RuleFor(x => x.BrandName)
            .NotEmpty()
            .WithMessage("Brand name is required.")
            .MaximumLength(100)
            .WithMessage("Brand name cannot be longer than 100 characters.");

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("Brand image is required.")
            .Must(ImageFileValidator.IsValid)
            .WithMessage(
                "Invalid image. Upload a valid JPEG, PNG, or WebP image no larger than 5 MB.");
    }
}