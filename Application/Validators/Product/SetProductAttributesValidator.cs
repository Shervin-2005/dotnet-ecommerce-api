using Application.DTOs.Product;
using FluentValidation;

namespace Application.Validators.Product;

public class SetProductAttributesValidator
    : AbstractValidator<SetProductAttributesDto>
{
    public SetProductAttributesValidator()
    {
        RuleFor(x => x.Attributes)
            .NotEmpty()
            .WithMessage("A product must have at least one attribute.")
            .Must(attributes => attributes.Count <= 5)
            .WithMessage("A product can have a maximum of 5 attributes.");

        RuleForEach(x => x.Attributes)
            .SetValidator(new CreateProductAttributeValidator());

        RuleFor(x => x.Attributes)
            .Must(HaveUniqueAttributeNames)
            .WithMessage("Duplicate attribute names are not allowed.");
    }

    private static bool HaveUniqueAttributeNames(
        List<CreateProductAttributeDto> attributes)
    {
        return attributes
            .Select(a => a.Name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() == attributes.Count;
    }
}