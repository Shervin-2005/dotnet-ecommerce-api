using Application.DTOs.Product;
using FluentValidation;

namespace Application.Validators.Product;

public class ProductSpecificationsValidator : AbstractValidator<List<UpsertProductSpecificationDto>>
{
    public ProductSpecificationsValidator()
    {
        RuleFor(x => x)
            .Must(specs => specs.Count <= 30)
            .WithMessage("A product can have a maximum of 30 specifications.");

        RuleFor(x => x)
            .Must(specs => specs.Count(s => s.IsMain) <= 10)
            .WithMessage("A product can have a maximum of 10 main specifications.");

        RuleForEach(x => x)
            .SetValidator(new UpsertProductSpecificationDtoValidator());
    }
}