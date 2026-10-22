using FluentValidation;
using dotnet_ecommerce_api.Models;

namespace dotnet_ecommerce_api.Validators;

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(150)
            .WithMessage("Product name cannot be longer than 150 characters.");

        RuleFor(x => x.OriginalPrice)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0.");

        RuleFor(x => x.SalePrice)
            .GreaterThan(0)
            .WithMessage("Sale price must be greater than 0.")
            .LessThanOrEqualTo(x => x.OriginalPrice)
            .WithMessage("Sale price cannot exceed the original price.")
            .When(x => x.SalePrice.HasValue);

        RuleFor(x => x.SaleDurationMinutes)
            .NotNull()
            .WithMessage("Sale duration is required when the sale price is lower than the original price.")
            .GreaterThan(0)
            .WithMessage("Sale duration must be greater than 0.")
            .When(x => x.SalePrice.HasValue && x.SalePrice.Value < x.OriginalPrice);

        RuleFor(x => x.SaleDurationMinutes)
            .Null()
            .WithMessage("Sale duration must not be specified when there is no discount.")
            .When(x => !x.SalePrice.HasValue || x.SalePrice.Value >= x.OriginalPrice);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(2000)
            .WithMessage("Description cannot be longer than 2000 characters.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity cannot be negative.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than 0.");

        RuleFor(x => x.BrandId)
            .GreaterThan(0)
            .WithMessage("Brand ID must be greater than 0.");

        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("At least one product image is required.");
        
        RuleForEach(x => x.Images)
            .Must(ImageFileValidator.IsValid)
            .WithMessage(
                "Each image must be a valid JPEG, PNG, or WebP image no larger than 5 MB.");
    }
}