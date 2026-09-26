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

        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("Category image is required.");

        RuleFor(x => x.ImageName)
            .NotEmpty()
            .WithMessage("Image name is required.")
            .MaximumLength(255)
            .WithMessage("Image name cannot be longer than 255 characters.");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Image content type is required.");
    }
}