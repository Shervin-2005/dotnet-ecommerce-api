using Application.DTOs.FlashSale;
using FluentValidation;

namespace Application.Validators.FlashSale;

public class AddFlashSaleItemDtoValidator : AbstractValidator<AddFlashSaleItemDto>
{
    public AddFlashSaleItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than zero.");

        RuleFor(x => x.DiscountPercentage)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Discount percentage must be between 0 and 100.");

        RuleForEach(x => x.ProductVariantIds)
            .GreaterThan(0)
            .WithMessage("Variant IDs must be greater than zero.");

        RuleFor(x => x.ProductVariantIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate variant IDs are not allowed.");
    }
}