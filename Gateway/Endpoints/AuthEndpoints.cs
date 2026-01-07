using Common.Contracts.Users;

namespace Gateway.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/login", async (IHttpClientFactory factory, LoginRequest request) =>
        {
            var client = factory.CreateClient("users");
            var res = await client.PostAsJsonAsync(
                "auth/login", request);
            //Сделать нормальную обработку ошибок
            if (!res.IsSuccessStatusCode)
                return Results.BadRequest(await res.Content.ReadAsStringAsync());
            
            var authResult = await res.Content.ReadFromJsonAsync<AuthResponce>();
            return Results.Ok(authResult);
        });

        app.MapPost("/users/register", async (IHttpClientFactory factory, RegisterRequest request) =>
        {
            var  client = factory.CreateClient("users");
            var res = await client.PostAsJsonAsync(
                "auth/register",  request);
            
            //Сделать нормальную обработку ошибок
            if (!res.IsSuccessStatusCode)
                return Results.BadRequest(await res.Content.ReadAsStringAsync());
            
            var registerResult = await res.Content.ReadFromJsonAsync<RegisterResponce>();
            return Results.Ok(registerResult);
        });
        
    }
}