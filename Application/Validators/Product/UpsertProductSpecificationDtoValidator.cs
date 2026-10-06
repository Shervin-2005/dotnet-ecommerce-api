using Application.DTOs.Product;
using FluentValidation;

public class UpsertProductSpecificationDtoValidator
    : AbstractValidator<UpsertProductSpecificationDto>
{
    public UpsertProductSpecificationDtoValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Value)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}