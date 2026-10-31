using FluentValidation;
using dotnet_ecommerce_api.Models;

namespace dotnet_ecommerce_api.Validators;

public class UpdateBrandRequestValidator : AbstractValidator<UpdateBrandRequest>
{
    public UpdateBrandRequestValidator()
    {
        RuleFor(x => x.BrandName)
            .NotEmpty()
            .WithMessage("Brand name is required.")
            .MaximumLength(100)
            .WithMessage("Brand name cannot be longer than 100 characters.");
        
        RuleFor(x => x.BrandGroupId)
            .GreaterThan(0)
            .When(x => x.BrandGroupId.HasValue)
            .WithMessage("Brand group ID must be greater than zero."); 

        When(x => x.File is not null, () =>
        {
            RuleFor(x => x.File)
                .Must(ImageFileValidator.IsValid)
                .WithMessage(
                    "Invalid image. Upload a valid JPEG, PNG, or WebP image no larger than 5 MB.");
        });
    }
}