using Application.DTOs.Category;
using FluentValidation;

namespace Application.Validators.Category;

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name cannot be longer than 100 characters.");

        RuleFor(x => x.ImageName)
            .NotEmpty()
            .WithMessage("Image name is required.")
            .MaximumLength(255)
            .When(x => x.Image is not null);

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .When(x => x.Image is not null);

        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0)
            .When(x => x.ParentCategoryId.HasValue)
            .WithMessage("Parent category ID must be greater than zero.");
    }
}