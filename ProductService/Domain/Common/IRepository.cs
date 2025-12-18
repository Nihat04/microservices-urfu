namespace ProductService.Domain.Common;

public interface IRepository<TAggregateRoot>
    where TAggregateRoot : class, IAggregateRoot
{
    public Task SaveChangesAsync(CancellationToken ct = default);
}
