using Common.Contracts;
using Common.Contracts.Products; 

namespace Gateway.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        //non-auth
        //all products
        app.MapGet("products", async (ServiceClient client) =>  
        await client.ForwardGetAsync<PaginatedResult<ProductResponse>>(
            "products", 
            $"/api/v1/prdoducts"));
        
        //auth
        //create product
        app.MapPost("products", async (ServiceClient client, ProductCreateRequest productCreateRequest) =>
            await client.ForwardPostAsync<ProductCreateRequest, ProductResponse>(
                "products",
                "/api/v1/products",
                productCreateRequest)).RequireAuthorization();
    }
    
}