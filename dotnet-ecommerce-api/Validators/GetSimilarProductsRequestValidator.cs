using dotnet_ecommerce_api.Models;
using FluentValidation;

namespace dotnet_ecommerce_api.Validators;

public class GetSimilarProductsRequestValidator : AbstractValidator<GetSimilarProductsRequest>
{
    public GetSimilarProductsRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than zero.");

        RuleFor(x => x.Count)
            .InclusiveBetween(1, 20)
            .WithMessage("Count must be between 1 and 20.");
    }
}