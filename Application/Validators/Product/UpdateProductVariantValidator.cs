using Application.DTOs.Product;
using FluentValidation;

namespace Application.Validators.Product;

public class UpdateProductVariantValidator
    : AbstractValidator<UpdateProductVariantDto>
{
    public UpdateProductVariantValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU is required.")
            .MaximumLength(100)
            .WithMessage("SKU cannot exceed 100 characters.");

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

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity cannot be negative.");

        RuleFor(x => x.AttributeValueIds)
            .NotEmpty()
            .WithMessage("A variant must have at least one attribute value.");

        RuleFor(x => x.AttributeValueIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate attribute values are not allowed.");
    }
}