using dotnet_ecommerce_api.Models;
using FluentValidation;

namespace dotnet_ecommerce_api.Validators;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name cannot be longer than 100 characters.");

        When(x => x.File is not null, () =>
        {
            RuleFor(x => x.File)
                .Must(ImageFileValidator.IsValid)
                .WithMessage(
                    "Invalid image. Upload a valid JPEG, PNG, or WebP image no larger than 5 MB.");
        });
    }
}