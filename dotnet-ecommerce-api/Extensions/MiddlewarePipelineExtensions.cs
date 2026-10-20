using Serilog;

namespace dotnet_ecommerce_api.Extensions;

public static class MiddlewarePipelineExtensions
{
    public const string ReactCorsPolicy = "ReactCorsPolicy";

    public static IServiceCollection AddReactCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(ReactCorsPolicy, policy =>
            {
                policy
                    .WithOrigins("http://localhost:3001")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");

            options.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors(ReactCorsPolicy);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}