using Application.Interfaces.SupportChat;
using Application.Mappings;
using dotnet_ecommerce_api.Extensions;
using dotnet_ecommerce_api.Hubs;
using dotnet_ecommerce_api.Middleware;
using dotnet_ecommerce_api.Services;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    Log.Information("Starting up dotnet-ecommerce-api");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

    var connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "DefaultConnection is not configured");

    builder.Services
        .AddPersistence(builder.Configuration)
        .AddApplicationServices()
        .AddInfrastructureServices(builder.Configuration)
        .AddJwtAuthentication(builder.Configuration)
        .AddSwaggerDocumentation()
        .AddHangfireServices(connectionString)
        .AddObservability()
        .AddValidation();

    // Controllers
    builder.Services.AddControllers();

    // SignalR
    builder.Services.AddSignalR();

    // CORS for React
    builder.Services.AddReactCors();

    // SignalR message notifications
    builder.Services.AddScoped<
        ISupportChatNotifier,
        SignalRSupportChatNotifier>();

    // Error handling
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    // AutoMapper
    builder.Services.AddAutoMapper(cfg =>
    {
        cfg.LicenseKey =
            builder.Configuration["AutoMapper:ServiceApiKey"];
    }, typeof(MappingProfile));

    var app = builder.Build();

    // Background jobs
    app.UseHangfireJobs();

    // HTTP middleware and controller endpoints
    app.UseApplicationMiddleware();

    // SignalR endpoint
    app.MapHub<SupportChatHub>("/hubs/support-chat");

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