using Application.DTOs.Order;
using FluentValidation;

namespace Application.Validators.Order;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.UserAddressId)
            .GreaterThan(0)
            .WithMessage("A valid user address must be selected.");
    }
}