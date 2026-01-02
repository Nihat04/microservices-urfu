using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using ProductService.Application.Extensions;
using ProductService.Infrastructure.Extensions;
using ProductService.Infrastructure.Storage;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//logger configuration
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("service", "product-service")
    .WriteTo.Console(
        outputTemplate:
        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<ServerDbContext>(config =>
{
    config.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    config.EnableSensitiveDataLogging();
});
builder.Services.RegisterInfrastructureLayer();
builder.Services.RegisterApplicationLayer();

builder.Services.AddOpenTelemetry().WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddPrometheusExporter());


builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ServerDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseOpenTelemetryPrometheusScrapingEndpoint("/metrics");

app.Run();
