using Application.DTOs.Cart;
using FluentValidation;

namespace Application.Validators.Cart;

public class AddToCartDtoValidator : AbstractValidator<AddToCartDto>
{
    public AddToCartDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than 0.");

        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 1000)
            .WithMessage("Quantity must be between 1 and 1000.");
    }
}