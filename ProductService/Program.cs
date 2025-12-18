using Microsoft.EntityFrameworkCore;
using ProductService.Application;
using ProductService.Infrastructure;
using ProductService.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ServerDbContext>(config =>
{
    config.UseNpgsql(builder.Configuration.GetConnectionString("DataBase"));
    config.EnableSensitiveDataLogging();
});
builder.Services.RegisterInfrastructureLayer();
builder.Services.RegisterApplicationLayer();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
