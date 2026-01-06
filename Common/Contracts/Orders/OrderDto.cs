using System.ComponentModel.DataAnnotations;

namespace Common.Contracts.Orders;

public record CreateOrderRequest(Guid UserId, IReadOnlyList<OrderItemDto> Items)
{
    public decimal CalculateTotalPrice() => Items.Sum(item => item.CalculateTotalPrice());
}

public record OrderResponse(
    Guid Id,
    Guid UserId,
    IReadOnlyList<OrderItemDto> Items,
    OrderState OrderState,
    DateTime CreatedAt,
    DateTime UpdatedAt
) : CreateOrderRequest(UserId, Items);

public record OrderItemDto(
    Guid ProductId,
    [Range(1, int.MaxValue)] decimal Quantity,
    [Range(1, 1000000)] decimal PricePerUnit
)
{
    public decimal CalculateTotalPrice() => Quantity * PricePerUnit;
}

public enum OrderState
{
    Created,
    Pending,
    Completed,
    Cancelled,
}
