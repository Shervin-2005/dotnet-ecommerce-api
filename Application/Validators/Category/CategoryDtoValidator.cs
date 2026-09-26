using Application.DTOs.Category;
using FluentValidation;

namespace Application.Validators.Category;

public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name cannot be longer than 100 characters.");

        RuleFor(x => x.MainImageUrl)
            .MaximumLength(500)
            .WithMessage("Main image URL cannot be longer than 500 characters.")
            .When(x => x.MainImageUrl is not null);
    }
}