using Common.Contracts.Orders;
using OrderService.Application.Infrastructure;
using OrderService.Domain.Common;

namespace OrderService.Domain.Entities;

public sealed class Order : Entity<Guid>, IAggregateRoot
{
    public Guid UserId;
    public IReadOnlyList<OrderItem> Items => _items;
    public OrderState State { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<OrderItem> _items = [];

    private Order()
        : base(Guid.Empty) { }

    public Order(Guid id, Guid userId, IEnumerable<OrderItem>? items = null)
        : base(id == Guid.Empty ? Guid.NewGuid() : id)
    {
        _items = (items != null) ? [.. items] : [];
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        State = OrderState.Created;
    }

    public static Order Create(Guid userId) => new(Guid.NewGuid(), userId);

    public OrderItem AddItem(Guid productId, decimal quantity, decimal pricePerUnit)
    {
        EnsureItemsEditable();

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            existing.UpdateQuantity(existing.Quantity + quantity);
            existing.UpdatePricePerUnit(pricePerUnit);
            UpdatedAt = DateTime.UtcNow;
            return existing;
        }

        var item = new OrderItem(productId, quantity, pricePerUnit);
        _items.Add(item);
        UpdatedAt = DateTime.UtcNow;
        return item;
    }

    public bool RemoveItem(Guid orderItemId)
    {
        EnsureItemsEditable();

        var item = _items.FirstOrDefault(i => i.Id == orderItemId);
        if (item == null)
            return false;
        _items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    public Result SetState(OrderState newState)
    {
        if (newState == State)
            return Error.Failure(
                $"Cannot set order state to the same value as the current one. Current state: {State}"
            );

        var allowed = State switch
        {
            OrderState.Created => [OrderState.Paid, OrderState.Cancelled],
            OrderState.Paid => [OrderState.Completed, OrderState.Cancelled],
            _ => Array.Empty<OrderState>(),
        };

        if (!allowed.Contains(newState))
            return Error.Failure($"Invalid state transition from {State} to {newState}.");

        State = newState;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    private void EnsureItemsEditable()
    {
        if (State != OrderState.Created)
            throw new InvalidOperationException($"Order cannot be modified when state is {State}.");
    }
}
