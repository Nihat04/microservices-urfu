using OrderService.Domain.Common;

namespace OrderService.Domain.Entities;

public sealed class OrderItem : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal PricePerUnit { get; private set; }
    public decimal TotalPrice => Quantity * PricePerUnit;

    private OrderItem()
        : base(Guid.Empty) { }

    public OrderItem(Guid productId, decimal quantity, decimal pricePerUnit)
        : base(Guid.NewGuid())
    {
        UpdateQuantity(quantity);
        UpdatePricePerUnit(pricePerUnit);
        ProductId = productId;
    }

    public void UpdateQuantity(decimal quantity)
    {
        if (quantity < 1)
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Products quantity must be greater than 0."
            );
        Quantity = quantity;
    }

    public void UpdatePricePerUnit(decimal pricePerUnit)
    {
        if (pricePerUnit < 1)
            throw new ArgumentOutOfRangeException(
                nameof(pricePerUnit),
                "Products price per unit must be greater than 0."
            );
        PricePerUnit = pricePerUnit;
    }
}
