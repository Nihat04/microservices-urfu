using System.Linq.Expressions;
using Common.Contracts;
using Common.Contracts.Orders;
using OrderService.Application.Extensions;
using OrderService.Application.Infrastructure;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Application;

public interface IOrderApplicationService
{
    Task<Result<Order>> CreateOrderAsync(OrderCreateRequest request, CancellationToken ct);
    Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Order>> GetOrdersByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct
    );
    Task<(IReadOnlyList<Order> items, int count)> GetOrdersAsync(
        Pagination pagination,
        OrderSortBy sortBy,
        SortOrder sortOrder,
        OrderFilterDto? filter,
        CancellationToken ct
    );
    Task<Result<Order>> PayOrderAsync(Guid id, CancellationToken ct);
    Task<Result<Order>> CompleteOrderAsync(Guid id, CancellationToken ct);
    Task<Result<Order>> CancelOrderAsync(Guid id, CancellationToken ct);
}

public sealed class OrderApplicationService(IOrderRepository orderRepository)
    : IOrderApplicationService
{
    public async Task<Result<Order>> CreateOrderAsync(
        OrderCreateRequest request,
        CancellationToken ct
    )
    {
        var order = new Order(
            new Guid(),
            request.UserId,
            request.Items.Select(item => item.ToEntity())
        );
        await orderRepository.AddAsync(order, ct);
        await orderRepository.UnitOfWork.SaveChangesAsync(ct);
        return order;
    }

    public Task<IReadOnlyList<Order>> GetOrdersByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct
    ) => orderRepository.ListAsync(p => ids.Contains(p.Id), ct);

    public async Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken ct) =>
        await orderRepository.FindAsync([id], ct);

    public async Task<(IReadOnlyList<Order> items, int count)> GetOrdersAsync(
        Pagination pagination,
        OrderSortBy sortBy,
        SortOrder sortOrder,
        OrderFilterDto? filter,
        CancellationToken ct
    )
    {
        var orderByExpression = GetOrderByExpression(sortBy);
        var query = await orderRepository.QueryAsync(
            (orders) =>
            {
                orders = ApplyFilter(orders, filter);
                if (sortOrder == SortOrder.Ascending)
                    orders = orders.OrderBy(orderByExpression);
                else
                    orders = orders.OrderByDescending(orderByExpression);
                var start = pagination.GetStart();
                return orders.Skip(start).Take(pagination.PageSize);
            },
            ct
        );
        var count = await orderRepository.CountQueryAsync(
            orders => ApplyFilter(orders, filter),
            ct
        );
        return (query, count);
    }

    public Task<Result<Order>> PayOrderAsync(Guid id, CancellationToken ct) =>
        UpdateOrderStateAsync(id, OrderState.Paid, ct);

    public Task<Result<Order>> CompleteOrderAsync(Guid id, CancellationToken ct) =>
        UpdateOrderStateAsync(id, OrderState.Completed, ct);

    public Task<Result<Order>> CancelOrderAsync(Guid id, CancellationToken ct) =>
        UpdateOrderStateAsync(id, OrderState.Cancelled, ct);

    private async Task<Result<Order>> UpdateOrderStateAsync(
        Guid id,
        OrderState state,
        CancellationToken ct
    )
    {
        var order = await orderRepository.FindAsync([id], ct);
        if (order == null)
            return Error.NotFound($"Order with id {id} not found");
        var result = order.SetState(state);
        if (result.IsError)
            return result.Error;
        await orderRepository.UnitOfWork.SaveChangesAsync(ct);
        return order;
    }

    private static Expression<Func<Order, object>> GetOrderByExpression(OrderSortBy orderBy) =>
        orderBy switch
        {
            OrderSortBy.CreateTime => o => o.CreatedAt,
            OrderSortBy.UpdateTime => o => o.UpdatedAt,
            _ => throw new ArgumentOutOfRangeException(nameof(orderBy)),
        };

    private static IQueryable<Order> ApplyFilter(IQueryable<Order> query, OrderFilterDto? filter)
    {
        if (filter == null)
            return query;

        if (filter.UserId != null)
            query = query.Where(o => o.UserId == filter.UserId);

        if (filter.CreatedFrom != null)
            query = query.Where(o => o.CreatedAt >= filter.CreatedFrom);

        if (filter.CreatedTo != null)
            query = query.Where(o => o.CreatedAt <= filter.CreatedTo);

        if (filter.UpdatedFrom != null)
            query = query.Where(o => o.UpdatedAt >= filter.UpdatedFrom);

        if (filter.UpdatedTo != null)
            query = query.Where(o => o.UpdatedAt <= filter.UpdatedTo);

        if (filter.State != null)
            query = query.Where(o => o.State == filter.State);

        return query;
    }
}
