using Application.Jobs;
using Hangfire;
using Hangfire.PostgreSql;

namespace dotnet_ecommerce_api.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireServices(this IServiceCollection services, string connectionString)
    {
        services.AddHangfire(config => config.UsePostgreSqlStorage(connectionString));
        services.AddHangfireServer();

        return services;
    }

    public static WebApplication UseHangfireJobs(this WebApplication app)
    {
        app.UseHangfireDashboard();

        using var scope = app.Services.CreateScope();
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<IOrderCleanupJob>(
            "cancel-expired-pending-orders",
            job => job.CancelExpiredPendingOrdersAsync(),
            "*/5 * * * *");

        recurringJobManager.AddOrUpdate<IOtpCleanupJob>(
            "delete-old-otps",
            job => job.DeleteExpiredOtpsAsync(),
            "0 3 * * *");

        return app;
    }
}