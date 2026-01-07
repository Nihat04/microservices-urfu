using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Polly;
using StackExchange.Redis;
using System.Text;
using Gateway.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------- Auth ----------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("super-secret-key"))
        };
    });

// ---------- Redis ----------
// builder.Services.AddSingleton<IConnectionMultiplexer>(
//        ConnectionMultiplexer.Connect("redis:6379"));

// ---------- HttpClients + Polly ----------
builder.Services.AddHttpClient("users", c =>
{
    c.BaseAddress = new Uri("http://user_service:8080/auth");
}).AddTransientHttpErrorPolicy(p =>
    p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(200)));

builder.Services.AddHttpClient("orders", c =>
{
    c.BaseAddress = new Uri("http://order_service:8080");
}).AddTransientHttpErrorPolicy(p =>
    p.CircuitBreakerAsync(3, TimeSpan.FromSeconds(30)));

builder.Services.AddHttpClient("products", c =>
{
    c.BaseAddress = new Uri("http://product_service:8080");
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); 
//app.UseAuthorization();

app.UseHttpsRedirection();
app.MapAuthEndpoints();
app.Run();

