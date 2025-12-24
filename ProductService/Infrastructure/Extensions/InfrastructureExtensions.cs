using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Storage.Repositories;

namespace ProductService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection RegisterInfrastructureLayer(this IServiceCollection services)
    {
        return services.RegisterRepositories();
    }

    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddTransient<IProductRepository, ProductRepository>();
        return services;
    }
}
