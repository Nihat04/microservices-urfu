using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OrderService.Application.Extensions;
using OrderService.Infrastructure.Extensions;
using OrderService.Infrastructure.Storage;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("service", "order-service")
    .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<ServerDbContext>(config =>
{
    config.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    config.EnableSensitiveDataLogging();
});
builder.Services.RegisterInfrastructureLayer();
builder.Services.RegisterApplicationLayer();

builder
    .Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation().AddPrometheusExporter());

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
