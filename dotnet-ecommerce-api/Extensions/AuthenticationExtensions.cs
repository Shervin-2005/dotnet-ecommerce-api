using System.Text;
using Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_ecommerce_api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        var jwtSettings = configuration
            .GetSection("JwtSettings")
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "JWT settings are not configured.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,

                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),

                        ClockSkew = TimeSpan.Zero
                    };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // SignalR may pass the token through
                        // the query string for WebSockets/SSE.
                        var accessToken = context.Request.Query["access_token"];

                        var requestPath = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && requestPath.StartsWithSegments("/hubs/support-chat"))
                        {
                            context.Token = accessToken;
                            return Task.CompletedTask;
                        }

                        // Normal API requests use the AccessToken cookie.
                        if (context.Request.Cookies.TryGetValue("AccessToken", out var cookieToken))
                        {
                            context.Token = cookieToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}