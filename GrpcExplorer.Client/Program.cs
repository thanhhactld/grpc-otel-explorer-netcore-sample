using BlogContracts;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using GrpcExplorer.OpenTelemetry;
using GrpcServer;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StatusCode = Grpc.Core.StatusCode;

var appBuilder = WebApplication.CreateBuilder(args);

// add logging
appBuilder.Services.AddSingleton<Instrumentation>();

#region OpenTelemetry

appBuilder.Services.AddOpenTelemetry()
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
        appBuilder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(
            appBuilder.Configuration.GetSection("AspNetCoreInstrumentation"));

        traceBuilder.AddOtlpExporter(otlpOptions =>
        {
            // Use IConfiguration directly for Otlp exporter endpoint option.
            otlpOptions.Endpoint =
                new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint",
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
                new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint",
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
                new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint",
                    defaultValue: "http://localhost:4317")!);
        });
    });

#endregion

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
appBuilder.Services.AddEndpointsApiExplorer();
appBuilder.Services.AddSwaggerGen();
appBuilder.Services.AddControllers();

appBuilder.Services.AddSingleton<BlogGrpc.BlogGrpcClient>(sp =>
{
    var defaultMethodConfig = new MethodConfig
    {
        Names = { MethodName.Default },
        RetryPolicy = new RetryPolicy
        {
            MaxAttempts = 10,
            InitialBackoff = TimeSpan.FromSeconds(1),
            MaxBackoff = TimeSpan.FromSeconds(5),
            BackoffMultiplier = 1.5,
            RetryableStatusCodes = { StatusCode.Unavailable }
        }
    };
    var channel = GrpcChannel.ForAddress("http://localhost:4000", new GrpcChannelOptions()
    {
        ServiceConfig = new ServiceConfig()
        {
            MethodConfigs = { defaultMethodConfig }
        },   
    });
    var client = new BlogGrpc.BlogGrpcClient(channel);
    return client;
});


appBuilder.Services.AddScoped<Greeter.GreeterClient>(sp =>
{
    var defaultMethodConfig = new MethodConfig
    {
        Names = { MethodName.Default },
        RetryPolicy = new RetryPolicy
        {
            MaxAttempts = 10,
            InitialBackoff = TimeSpan.FromSeconds(1),
            MaxBackoff = TimeSpan.FromSeconds(5),
            BackoffMultiplier = 1.5,
            RetryableStatusCodes = { StatusCode.Unavailable }
        }
    };
    var channel = GrpcChannel.ForAddress("http://localhost:5282", new GrpcChannelOptions()
    {
        ServiceConfig = new ServiceConfig()
        {
            MethodConfigs = { defaultMethodConfig }
        },
    });
    var client = new Greeter.GreeterClient(channel);
    return client;
});


var app = appBuilder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();
