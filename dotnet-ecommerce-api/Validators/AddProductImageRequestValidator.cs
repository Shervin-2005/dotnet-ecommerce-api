using FluentValidation;
using dotnet_ecommerce_api.Models;

namespace dotnet_ecommerce_api.Validators;

public class AddProductImageRequestValidator : AbstractValidator<AddProductImageRequest>
{
    public AddProductImageRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("An image is required.")
            .Must(ImageFileValidator.IsValid)
            .WithMessage("Invalid image. Upload a valid JPEG, PNG, or WebP image no larger than 5 MB.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}