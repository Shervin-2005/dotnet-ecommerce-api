using Serilog;

namespace dotnet_ecommerce_api.Extensions;

public static class MiddlewarePipelineExtensions
{
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
        
        app.UseCors(options => options
            .WithOrigins("http://localhost:3001") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}