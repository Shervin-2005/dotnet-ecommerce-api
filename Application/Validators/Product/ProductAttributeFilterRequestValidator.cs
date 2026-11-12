using Application.DTOs.Product;
using FluentValidation;

namespace Application.Validators.Product;

public class ProductAttributeFilterRequestValidator
    : AbstractValidator<ProductAttributeFilterRequest>
{
    public ProductAttributeFilterRequestValidator()
    {
        RuleFor(x => x.AttributeName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Values)
            .NotNull()
            .NotEmpty()
            .Must(values => values.Count <= 10)
            .WithMessage("An attribute cannot have more than 10 selected values.");

        RuleForEach(x => x.Values)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Values)
            .Must(values => values == null ||
                            values.Distinct(StringComparer.OrdinalIgnoreCase).Count() == values.Count)
            .WithMessage("Duplicate attribute values are not allowed.");
    }
}