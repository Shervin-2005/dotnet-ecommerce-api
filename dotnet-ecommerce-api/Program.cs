using Application.Mappings;
using dotnet_ecommerce_api.Extensions;
using dotnet_ecommerce_api.Middleware;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up dotnet-ecommerce-api");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? throw new InvalidOperationException("DefaultConnection is not configured");

    builder.Services
        .AddPersistence(builder.Configuration)
        .AddApplicationServices()
        .AddInfrastructureServices(builder.Configuration)
        .AddJwtAuthentication(builder.Configuration)
        .AddSwaggerDocumentation()
        .AddHangfireServices(connectionString)
        .AddObservability()
        .AddValidation();

    builder.Services.AddControllers();
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddAutoMapper(cfg =>
    {
        cfg.LicenseKey = builder.Configuration["AutoMapper:ServiceApiKey"];
    }, typeof(MappingProfile));

    var app = builder.Build();

    app.UseHangfireJobs();
    app.UseApplicationMiddleware();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}