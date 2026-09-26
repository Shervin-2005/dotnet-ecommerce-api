using dotnet_ecommerce_api.Models;
using FluentValidation;

namespace dotnet_ecommerce_api.Validators;

public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name cannot be longer than 100 characters.");

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("Category image is required.");
    }
}