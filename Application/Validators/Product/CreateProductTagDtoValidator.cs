using Application.DTOs.Product;
using FluentValidation;

namespace Application.Validators.Product;

public class CreateProductTagDtoValidator : AbstractValidator<CreateProductTagDto>
{
    public CreateProductTagDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}