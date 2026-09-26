using FluentValidation;
using Application.DTOs.Product;

namespace Application.Validators.Product;

public class ProductImageDtoValidator : AbstractValidator<ProductImageDto>
{
    public ProductImageDtoValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("Product image is required.");

        RuleFor(x => x.ImageName)
            .NotEmpty()
            .WithMessage("Image name is required.")
            .MaximumLength(255)
            .WithMessage("Image name cannot be longer than 255 characters.");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Image content type is required.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order cannot be negative.");
    }
}