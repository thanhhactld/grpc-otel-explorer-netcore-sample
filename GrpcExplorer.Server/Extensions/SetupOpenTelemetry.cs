using GrpcExplorer.OpenTelemetry;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace GrpcExplorer.Server.Extensions;

public static class SetupOpenTelemetry
{
    public static IServiceCollection SetupOpenTelemetryServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(
                    serviceName: typeof(Program).Assembly.GetName().Name!,
                    serviceVersion: "v1",
                    serviceInstanceId: Environment.MachineName))
            .WithTracing(traceBuilder =>
            {
                // Tracing

                // Ensure the TracerProvider subscribes to any custom ActivitySources.
                traceBuilder
                    .AddSource(Instrumentation.ActivitySourceName)
                    .SetSampler(new AlwaysOnSampler())
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                // Use IConfiguration binding for AspNetCore instrumentation options.
                services.Configure<AspNetCoreTraceInstrumentationOptions>(
                    configuration.GetSection("AspNetCoreInstrumentation"));

                traceBuilder.AddOtlpExporter(otlpOptions =>
                {
                    // Use IConfiguration directly for Otlp exporter endpoint option.
                    otlpOptions.Endpoint =
                        new Uri(configuration.GetValue("Otlp:Endpoint",
                            defaultValue: "http://localhost:4317")!);
                });
            })
            .WithMetrics(metricBuilder =>
            {
                // Metrics

                // Ensure the MeterProvider subscribes to any custom Meters.
                metricBuilder
                    .AddMeter(Instrumentation.MeterName)
                    .SetExemplarFilter(ExemplarFilterType.TraceBased)
                    // .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                metricBuilder.AddOtlpExporter(otlpOptions =>
                {
                    // Use IConfiguration directly for Otlp exporter endpoint option.
                    otlpOptions.Endpoint =
                        new Uri(configuration.GetValue("Otlp:Endpoint",
                            defaultValue: "http://localhost:4317")!);
                });
            })
            .WithLogging(logBuilder =>
            {
                // Note: See appsettings.json Logging:OpenTelemetry section for configuration.
                logBuilder.AddOtlpExporter(otlpOptions =>
                {
                    // Use IConfiguration directly for Otlp exporter endpoint option.
                    otlpOptions.Endpoint =
                        new Uri(configuration.GetValue("Otlp:Endpoint",
                            defaultValue: "http://localhost:4317")!);
                });
            });

        return services;
    }
}