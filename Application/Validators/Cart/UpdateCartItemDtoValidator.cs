using Application.DTOs.Cart;
using FluentValidation;

namespace Application.Validators.Cart;

public class UpdateCartItemDtoValidator : AbstractValidator<UpdateCartItemDto>
{
    public UpdateCartItemDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 1000)
            .WithMessage("Quantity must be between 1 and 1000.");
    }
}