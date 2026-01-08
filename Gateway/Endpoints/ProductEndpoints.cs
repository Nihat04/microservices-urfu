using Common.Contracts;
using Common.Contracts.Products; 

namespace Gateway.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        //non-auth
        //all products
        app.MapGet("/products", async (ServiceClient client) =>
            {
                var res = await client.ForwardGetAsync<PaginatedResult<ProductResponse>>(
                    "products",
                    "/api/v1/products/list"
                ); 
                return res;
            }
            
        );
            
        //auth
        //create product
        app.MapPost("products", async (ServiceClient client, ProductCreateRequest productCreateRequest) =>
            await client.ForwardPostAsync<ProductCreateRequest, ProductResponse>(
                "products",
                "/api/v1/products",
                productCreateRequest));
    }
    
}