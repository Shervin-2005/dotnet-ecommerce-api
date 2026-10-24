using Application.DTOs.Product;
using FluentValidation;

namespace Application.Validators.Product;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(150)
            .WithMessage("Product name cannot be longer than 150 characters.");

        RuleFor(x => x.OriginalPrice)
            .GreaterThan(0)
            .WithMessage("Original price must be greater than zero.");

        RuleFor(x => x.SalePrice)
            .GreaterThan(0)
            .When(x => x.SalePrice.HasValue)
            .WithMessage("Sale price must be greater than zero.");

        RuleFor(x => x.SalePrice)
            .LessThanOrEqualTo(x => x.OriginalPrice)
            .When(x => x.SalePrice.HasValue)
            .WithMessage("Sale price cannot be greater than original price.");

        RuleFor(x => x.SaleDurationMinutes)
            .NotNull()
            .When(x =>
                x.SalePrice.HasValue &&
                x.SalePrice.Value < x.OriginalPrice)
            .WithMessage(
                "Sale duration is required when the sale price is lower than the original price.");

        RuleFor(x => x.SaleDurationMinutes)
            .GreaterThan(0)
            .When(x => x.SaleDurationMinutes.HasValue)
            .WithMessage("Sale duration must be greater than zero.");

        RuleFor(x => x.SaleDurationMinutes)
            .Null()
            .When(x =>
                !x.SalePrice.HasValue ||
                x.SalePrice.Value == x.OriginalPrice)
            .WithMessage(
                "Sale duration can only be specified when the sale price is lower than the original price.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(5000)
            .WithMessage("Description cannot be longer than 5000 characters.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity cannot be negative.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than zero.");

        RuleFor(x => x.BrandId)
            .GreaterThan(0)
            .WithMessage("Brand ID must be greater than zero.");

        RuleFor(x => x.Images)
            .NotNull()
            .WithMessage("Images cannot be null.");
    }
}