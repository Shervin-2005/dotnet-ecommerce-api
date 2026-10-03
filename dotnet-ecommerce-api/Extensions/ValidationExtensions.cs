using Application.Validators.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace dotnet_ecommerce_api.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<VerifyRegistrationOtpDtoValidator>();

        return services;
    }
}