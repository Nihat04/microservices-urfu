using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Polly;
using StackExchange.Redis;
using System.Text;
using Gateway;
using Gateway.Endpoints;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.IncludeFields = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddScoped<ServiceClient>();

// ---------- Auth ----------
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes("secretKey1234567890secretKey1234567890");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

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
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapAuthEndpoints();
app.MapOrderEndpoints();
app.MapProductEndpoints();
app.Run();

