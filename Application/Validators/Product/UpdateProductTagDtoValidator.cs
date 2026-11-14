using Application.DTOs.Product;
using FluentValidation;

namespace Application.Validators.Product;

public class UpdateProductTagDtoValidator : AbstractValidator<UpdateProductTagDto>
{
    public UpdateProductTagDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}