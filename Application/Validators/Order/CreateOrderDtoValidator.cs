using Application.DTOs.Order;
using FluentValidation;

namespace Application.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.ShippingAddress)
            .MaximumLength(500)
            .WithMessage("Shipping address cannot be longer than 500 characters.")
            .When(x => x.ShippingAddress is not null);
    }
}