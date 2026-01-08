using Common.Contracts.Users;
using Gateway;

namespace Gateway.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/login", async (ServiceClient client, LoginRequest request) => 
            await client.ForwardPostAsync<LoginRequest, AuthResponce>("users", "auth/login", request));

        app.MapPost("/users/register", async (ServiceClient client, RegisterRequest request) =>
            await client.ForwardPostAsync<RegisterRequest, RegisterResponce>("users", "auth/register", request));
    }
}