using Common.Contracts;
using Common.Contracts.Orders;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application;
using OrderService.Application.Extensions;
using OrderService.Application.Infrastructure;

namespace OrderService.Api;

[ApiController]
[Route(ApiConstants.ApiRoute)]
public class OrdersController(IOrderApplicationService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        OrderCreateRequest request,
        CancellationToken ct
    )
    {
        var result = await orderService.CreateOrderAsync(request, ct);
        return result.ToActionResult(this, o => o.ToResponse());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetOrderById(Guid id, CancellationToken ct)
    {
        var order = await orderService.GetOrderByIdAsync(id, ct);
        return order is null ? NotFound() : Ok(order.ToResponse());
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersByIds([FromQuery] Guid[] ids, CancellationToken ct)
    {
        if (ids.Length == 0)
            return Ok(Array.Empty<OrderResponse>());
        var orders = await orderService.GetOrdersByIdsAsync(ids, ct);
        return Ok(orders.Select(o => o!.ToResponse()));
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetOrders(
        [FromQuery] Pagination pagination,
        [FromQuery] OrderSortBy sortBy = OrderSortBy.CreateTime,
        [FromQuery] SortOrder sortOrder = SortOrder.Ascending,
        [FromQuery] OrderFilterDto? filter = default,
        CancellationToken ct = default
    )
    {
        var (orders, count) = await orderService.GetOrdersAsync(
            pagination: pagination,
            sortBy: sortBy,
            sortOrder: sortOrder,
            filter: filter,
            ct
        );
        var ordersDto = orders.Select(o => o.ToResponse()).ToArray();
        var response = new PaginatedResult<OrderResponse>(ordersDto, pagination, count);
        return Ok(response);
    }

    [HttpPost("pay/{id:guid}")]
    public async Task<IActionResult> PayOrder(Guid id, CancellationToken ct)
    {
        var result = await orderService.PayOrderAsync(id, ct);
        return result.ToActionResult(this, o => o.ToResponse());
    }

    [HttpPost("complete/{id:guid}")]
    public async Task<IActionResult> CompliteOrder(Guid id, CancellationToken ct)
    {
        var result = await orderService.CompleteOrderAsync(id, ct);
        return result.ToActionResult(this, o => o.ToResponse());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken ct)
    {
        var result = await orderService.CancelOrderAsync(id, ct);
        return result.ToActionResult(this, o => o.ToResponse());
    }
}
