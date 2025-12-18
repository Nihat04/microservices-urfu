using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.DataStorage;

namespace ProductService.Infrastructure.Storage.Repositories;

public class ProductRepository(ServerDbContext context)
    : EFRepository<Product, ServerDbContext>(context),
        IProductRepository { }
