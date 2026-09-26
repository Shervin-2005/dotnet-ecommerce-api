using Application.DTOs.Brand;
using FluentValidation;

namespace Application.Validators.Brand;

public class CreateBrandDtoValidator : AbstractValidator<CreateBrandDto>
{
    public CreateBrandDtoValidator()
    {
        RuleFor(x => x.BrandName)
            .NotEmpty()
            .WithMessage("Brand name is required.")
            .MaximumLength(100)
            .WithMessage("Brand name cannot be longer than 100 characters.");

        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("Brand image is required.");

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