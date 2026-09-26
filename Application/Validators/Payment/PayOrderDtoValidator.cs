using Application.DTOs.Payment;
using FluentValidation;

namespace Application.Validators.Payment;

public class PayOrderDtoValidator : AbstractValidator<PayOrderDto>
{
    public PayOrderDtoValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .WithMessage("Card number is required.")
            .CreditCard()
            .WithMessage("Please enter a valid card number.");

        RuleFor(x => x.CardHolderName)
            .NotEmpty()
            .WithMessage("Cardholder name is required.")
            .MaximumLength(100)
            .WithMessage("Cardholder name cannot be longer than 100 characters.");

        RuleFor(x => x.ExpiryMonth)
            .NotEmpty()
            .WithMessage("Expiry month is required.")
            .Matches(@"^(0[1-9]|1[0-2])$")
            .WithMessage("Expiry month must be 01-12.");

        RuleFor(x => x.ExpiryYear)
            .NotEmpty()
            .WithMessage("Expiry year is required.")
            .Matches(@"^\d{4}$")
            .WithMessage("Expiry year must be 4 digits.");

        RuleFor(x => x.Cvv)
            .NotEmpty()
            .WithMessage("CVV is required.")
            .Matches(@"^\d{3,4}$")
            .WithMessage("CVV must be 3 or 4 digits.");
    }
}