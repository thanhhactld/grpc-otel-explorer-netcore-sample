using GrpcExplorer.OpenTelemetry;
using GrpcExplorer.Server;
using GrpcExplorer.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Setup opentelemetry

builder.Services.AddSingleton<Instrumentation>();

builder.Services.SetupOpenTelemetryServices(builder.Configuration);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();