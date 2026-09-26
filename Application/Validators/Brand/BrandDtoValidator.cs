using Application.DTOs.Brand;
using FluentValidation;

namespace Application.Validators.Brand;

public class BrandDtoValidator : AbstractValidator<BrandDto>
{
    public BrandDtoValidator()
    {
        RuleFor(x => x.BrandName)
            .NotEmpty()
            .WithMessage("Brand name is required.")
            .MaximumLength(100)
            .WithMessage("Brand name cannot be longer than 100 characters.");

        RuleFor(x => x.MainImageUrl)
            .NotEmpty()
            .WithMessage("Main image URL is required.")
            .MaximumLength(500)
            .WithMessage("Main image URL cannot be longer than 500 characters.");
    }
}