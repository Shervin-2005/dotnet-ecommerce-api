using Application.DTOs.Product;
using FluentValidation;

namespace Application.Validators.Product;

public class CreateProductAttributeValidator
    : AbstractValidator<CreateProductAttributeDto>
{
    public CreateProductAttributeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Attribute name is required.")
            .MaximumLength(100)
            .WithMessage("Attribute name cannot exceed 100 characters.");

        RuleFor(x => x.Values)
            .NotEmpty()
            .WithMessage("An attribute must have at least one value.")
            .Must(values => values.Count <= 10)
            .WithMessage("An attribute can have a maximum of 10 values.");

        RuleForEach(x => x.Values)
            .NotEmpty()
            .WithMessage("Attribute values cannot be empty.")
            .MaximumLength(100)
            .WithMessage("Attribute values cannot exceed 100 characters.");

        RuleFor(x => x.Values)
            .Must(HaveUniqueValues)
            .WithMessage("Duplicate attribute values are not allowed.");
    }

    private static bool HaveUniqueValues(List<string> values)
    {
        return values
            .Select(v => v.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() == values.Count;
    }
}