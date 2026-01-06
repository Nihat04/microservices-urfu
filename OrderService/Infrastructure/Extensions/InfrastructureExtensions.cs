using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Storage.Repositories;

namespace OrderService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection RegisterInfrastructureLayer(this IServiceCollection services)
    {
        return services.RegisterRepositories();
    }

    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddTransient<IOrderRepository, OrderRepository>();
        return services;
    }
}
