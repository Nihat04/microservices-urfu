using Common.Contracts.Orders;
using OrderService.Domain.Entities;

namespace OrderService.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection RegisterApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IOrderApplicationService, OrderApplicationService>();
        return services;
    }

    public static OrderItem ToEntity(this OrderItemDto itemDto) =>
        new(itemDto.ProductId, itemDto.Quantity, itemDto.PricePerUnit);

    public static OrderItemDto ToDto(this OrderItem item) =>
        new(item.ProductId, item.Quantity, item.PricePerUnit);

    public static OrderResponse ToResponse(this Order order) =>
        new(
            order.Id,
            order.UserId,
            [.. order.Items.Select(item => item.ToDto())],
            order.State,
            order.CreatedAt,
            order.UpdatedAt
        );
}
