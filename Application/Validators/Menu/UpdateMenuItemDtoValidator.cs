using Application.DTOs.Menu;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Menu;

public class UpdateMenuItemDtoValidator : AbstractValidator<UpdateMenuItemDto>
{
    public UpdateMenuItemDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ParentMenuItemId)
            .GreaterThan(0)
            .When(x => x.ParentMenuItemId.HasValue);

        RuleFor(x => x.Type)
            .IsInEnum();

        When(x => x.Type == MenuItemType.Category, () =>
        {
            RuleFor(x => x.CategoryId)
                .NotNull()
                .GreaterThan(0);
            
            RuleFor(x => x.BrandId)
                .Null();

            RuleFor(x => x.TagId)
                .Null();

            RuleFor(x => x.Url)
                .Null();
        });
        
        // Brand navigation
        When(x => x.Type == MenuItemType.Brand, () =>
        {
            RuleFor(x => x.BrandId)
                .NotNull()
                .GreaterThan(0);

            RuleFor(x => x.CategoryId)
                .Null();
            
            RuleFor(x => x.TagId)
                .Null();

            RuleFor(x => x.Url)
                .Null();
        });

        When(x => x.Type == MenuItemType.Tag, () =>
        {
            RuleFor(x => x.TagId)
                .NotNull()
                .GreaterThan(0);

            RuleFor(x => x.CategoryId)
                .Null();
            
            RuleFor(x => x.BrandId)
                .Null();

            RuleFor(x => x.Url)
                .Null();
        });

        When(x => x.Type == MenuItemType.CustomUrl, () =>
        {
            RuleFor(x => x.Url)
                .NotEmpty()
                .MaximumLength(2048)
                .Must(BeValidUrl)
                .WithMessage("URL must be a valid HTTP, HTTPS, or relative site URL.");

            RuleFor(x => x.CategoryId)
                .Null();
            
            RuleFor(x => x.BrandId)
                .Null();

            RuleFor(x => x.TagId)
                .Null();
        });

        When(x => x.Type == MenuItemType.None, () =>
        {
            RuleFor(x => x.CategoryId)
                .Null();
            
            RuleFor(x => x.BrandId)
                .Null();
            
            RuleFor(x => x.TagId)
                .Null();

            RuleFor(x => x.Url)
                .Null();
        });
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        if (url.StartsWith('/') && !url.StartsWith("//"))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}