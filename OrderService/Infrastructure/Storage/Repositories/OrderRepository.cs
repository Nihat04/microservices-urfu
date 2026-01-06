using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Infrastructure.Storage.Repositories;

public class OrderRepository(ServerDbContext context)
    : EFRepository<Order, ServerDbContext>(context),
        IOrderRepository { }
