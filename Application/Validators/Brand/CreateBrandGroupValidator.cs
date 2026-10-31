using Application.DTOs.Brand;
using FluentValidation;

namespace Application.Validators.Brand;

public class CreateBrandGroupValidator : AbstractValidator<CreateBrandGroupDto>
{
    public CreateBrandGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}