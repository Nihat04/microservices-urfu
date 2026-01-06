using OrderService.Domain.Common;

namespace OrderService.Domain.Entities;

public sealed class Order : Entity<Guid>, IAggregateRoot
{
    public DateTime UpdatedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order()
        : base(Guid.Empty) { }

    public Order(Guid id)
        : base(id)
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Order Create()
    {
        var order = new Order(id: Guid.NewGuid());
        return order;
    }
}
