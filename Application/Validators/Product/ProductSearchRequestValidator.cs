using Application.DTOs.Product;
using FluentValidation;

namespace Application.Validators.Product;

public class ProductSearchRequestValidator
    : AbstractValidator<ProductSearchRequest>
{
    public ProductSearchRequestValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(100)
            .When(x => x.Search is not null);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.BrandId)
            .GreaterThan(0)
            .When(x => x.BrandId.HasValue);

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x)
            .Must(x =>
                !x.MinPrice.HasValue ||
                !x.MaxPrice.HasValue ||
                x.MinPrice.Value <= x.MaxPrice.Value)
            .WithMessage("Minimum price cannot exceed maximum price.");

        RuleFor(x => x.SortBy)
            .Must(value =>
                !string.IsNullOrWhiteSpace(value) &&
                new[] { "relevance", "name", "price" }
                    .Contains(value, StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortBy must be relevance, name, or price.");

        RuleFor(x => x.SortOrder)
            .Must(value =>
                !string.IsNullOrWhiteSpace(value) &&
                new[] { "asc", "desc" }
                    .Contains(value, StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortOrder must be asc or desc.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50);
    }
}