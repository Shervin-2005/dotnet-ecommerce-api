using Application.DTOs.Brand;
using FluentValidation;

namespace Application.Validators.Brand;

public class UpdateBrandGroupValidator : AbstractValidator<UpdateBrandGroupDto>
{
    public UpdateBrandGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}