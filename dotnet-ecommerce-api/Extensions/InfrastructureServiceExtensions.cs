using Amazon.Runtime;
using Amazon.S3;
using Application.Interfaces;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace dotnet_ecommerce_api.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IImageStorageService, S3FileStorageService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPaymentGateway, MockPaymentGateway>();
        services.AddHttpClient();

        services.Configure<S3Settings>(configuration.GetSection("ArvanStorage"));
        services.Configure<SmsSettings>(configuration.GetSection("OTPOptions"));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
            var credentials = new BasicAWSCredentials(settings.AccessKey, settings.SecretKey);

            var config = new AmazonS3Config
            {
                ServiceURL = settings.ServiceUrl,
                ForcePathStyle = true
            };

            return new AmazonS3Client(credentials, config);
        });

        return services;
    }
}