using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Services;
using UserService.Data;
using UserService.Repository;
using LoginRequest = Common.Contracts.Users.LoginRequest;
using RegisterRequest = Common.Contracts.Users.RegisterRequest;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


app.UseHttpsRedirection();

app.MapGet("/health", () => "Healthy");
app.MapPost("/auth/register", async (
    RegisterRequest request,
    AuthService authService) =>
{
    var user = await authService.RegisterAsync(request);

    return Results.Ok(new
    {
        user.Id,
        user.Email,
        user.Name
    });
});
app.MapPost("/auth/login", async (
    LoginRequest request,
    AuthService authService) =>
{
    var result = await authService.LoginAsync(request);

    if (result == null)
        return Results.Unauthorized();

    return Results.Ok(result);
});

app.Run();
