using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace dotnet_ecommerce_api.Extensions;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("dotnet-ecommerce-api"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/traces");
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("Microsoft.EntityFrameworkCore")
                    .AddOtlpExporter((options, metricReaderOptions) =>
                    {
                        options.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/metrics");
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        metricReaderOptions.TemporalityPreference = MetricReaderTemporalityPreference.Delta;
                    });
            });

        return services;
    }
}