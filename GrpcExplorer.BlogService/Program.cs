using GrpcExplorer.BlogService;
using GrpcExplorer.BlogService.Controllers;
using GrpcExplorer.BlogService.EntityFramework;
using GrpcExplorer.BlogService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BloggingContext>(opts =>
{
    opts.UseNpgsql(configuration.GetConnectionString("Default"));
});

// open telemetry
builder.Services.SetupOpenTelemetryServices(configuration: builder.Configuration);

builder.Services.AddScoped<BlogService>();

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<BloggingGrpcService>();
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}