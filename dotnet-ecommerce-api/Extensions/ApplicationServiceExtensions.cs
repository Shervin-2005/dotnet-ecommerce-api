using Application.Interfaces;
using Application.Jobs;
using Application.Services;
using Application.Settings;

namespace dotnet_ecommerce_api.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductVariantService, ProductVariantService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IOtpCleanupJob, OtpCleanupJob>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserAddressService, UserAddressService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderCleanupJob, OrderCleanupJob>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IOfferCodeService, OfferCodeService>();
        services.AddScoped<ISupportChatService, SupportChatService>();
        services.AddScoped<ISupportChatCleanupJob, SupportChatCleanupJob>();
        
        services.AddOptions<DefaultImagesOptions>().BindConfiguration("DefaultImages")
            .Validate(options => Uri.TryCreate(
                                     options.Profile,
                                     UriKind.Absolute,
                                     out var uri) &&
                                 uri.Scheme == Uri.UriSchemeHttps,
                "The default profile image URL must be a valid HTTPS URL.")
            .ValidateOnStart();
        
        return services;
    }
}