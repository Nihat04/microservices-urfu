using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gateway;

public class ServiceClient(IHttpClientFactory factory)
{
    public async Task<IResult> ForwardPostAsync<TRequest, TResponse>(
        string clientName, 
        string endpoint, 
        TRequest request)
    {
        var client = factory.CreateClient(clientName);
        var response = await client.PostAsJsonAsync(endpoint, request);
        
        if (!response.IsSuccessStatusCode)
            return Results.BadRequest(await response.Content.ReadAsStringAsync());
        
        // Настройки десериализации
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        
        var result = await response.Content.ReadFromJsonAsync<TResponse>(options);
        return Results.Ok(result);
    }

    public async Task<IResult> ForwardGetAsync<TResponse>(
        string clientName,
        string endpoint)
    {
        var client = factory.CreateClient(clientName);
        var response = await client.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
            return Results.Problem(await response.Content.ReadAsStringAsync());

        // Настройки десериализации
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        var result = await response.Content.ReadFromJsonAsync<TResponse>(options);
        return Results.Ok(result);
    }
}