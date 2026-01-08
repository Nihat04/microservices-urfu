using Common.Contracts;
using Common.Contracts.Orders;
using Microsoft.AspNetCore.Mvc;


namespace Gateway.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        //auth
        //Создаем заказ, меняем остатки
        app.MapPost("create-order", async (ServiceClient client, OrderCreateRequest request) =>
        {
            //Меняем остатки
            foreach (var item in request.Items)
            {
                var productId = item.ProductId;
                var quantity = item.Quantity;

                var response = await client.ForwardPostAsync<object, object>(
                    "orders",
                    $"/api/v1/products/{productId}/book?quantity={quantity}",
                    null);
            }

            var creatingOrderResponce = await client.ForwardPostAsync<OrderCreateRequest, OrderResponse>(
                "orders",
                "api/v1/orders",
                request);

            return creatingOrderResponce;
        });
        
        //auth-admin
        //Все заказы
        app.MapGet("get-orders", async (ServiceClient client) => 
            await client.ForwardGetAsync<PaginatedResult<OrderResponse>>(
                "orders", 
                $"/api/v1/orders/list"));


    }
}