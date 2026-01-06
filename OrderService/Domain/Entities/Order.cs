using Common.Contracts.Orders;
using OrderService.Domain.Common;

namespace OrderService.Domain.Entities;

public sealed class Order : Entity<Guid>, IAggregateRoot
{
    public IReadOnlyList<OrderItem> Items => _items;
    public DateTime UpdatedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderState State { get; private set; }

    private readonly List<OrderItem> _items = [];

    private Order()
        : base(Guid.Empty) { }

    public Order(Guid id, IEnumerable<OrderItem>? items = null)
        : base(id == Guid.Empty ? Guid.NewGuid() : id)
    {
        _items = (items != null) ? [.. items] : [];
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        State = OrderState.Created;
    }

    public static Order Create() => new(Guid.NewGuid());

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

    public void SetState(OrderState newState)
    {
        if (newState == State)
            return;
        State = newState;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureItemsEditable()
    {
        if (State != OrderState.Created)
            throw new InvalidOperationException($"Order cannot be modified when state is {State}.");
    }
}
