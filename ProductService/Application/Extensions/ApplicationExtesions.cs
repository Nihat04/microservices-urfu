namespace ProductService.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection RegisterApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IProductApplicationService, ProductApplicationService>();
        return services;
    }
}
