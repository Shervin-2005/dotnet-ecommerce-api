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

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0.");

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
    }
}