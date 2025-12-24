using System.Linq.Expressions;

namespace ProductService.Domain.Common;

public interface IRepository<TAggregateRoot>
    where TAggregateRoot : class, IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
    ValueTask<TAggregateRoot?> FindAsync(object[] keys, CancellationToken cancellationToken);
    Task<TAggregateRoot?> FirstOrDefaultAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken
    );
    Task<TAggregateRoot?> FirstOrDefaultAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        Expression<Func<TAggregateRoot, object>> orderBy,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<TAggregateRoot>> ListAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<TAggregateRoot>> ListAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        Expression<Func<TAggregateRoot, object>> orderBy,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<TAggregateRoot>> ListAsync(
        Paginating paginating,
        Expression<Func<TAggregateRoot, object>> orderBy,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<TAggregateRoot>> ListAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        Paginating paginating,
        Expression<Func<TAggregateRoot, object>> orderBy,
        CancellationToken cancellationToken
    );
    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<int> CountAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken
    );
    public Task<int> CountQueryAsync(
        Func<IQueryable<TAggregateRoot>, IQueryable<TAggregateRoot>> predicate,
        CancellationToken cancellationToken
    );
    ValueTask<TAggregateRoot> AddAsync(
        TAggregateRoot aggregateRoot,
        CancellationToken cancellationToken
    );
    Task AddRangeAsync(
        IReadOnlyList<TAggregateRoot> aggregateRoots,
        CancellationToken cancellationToken
    );
    Task RemoveAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken);
    Task RemoveRangeAsync(
        IReadOnlyList<TAggregateRoot> aggregateRoots,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<TResult>> QueryAsync<TResult>(
        Func<IQueryable<TAggregateRoot>, IQueryable<TResult>> predicate,
        CancellationToken cancellationToken
    );
}
