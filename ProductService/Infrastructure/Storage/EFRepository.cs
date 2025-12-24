using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Common;

namespace ProductService.Infrastructure.Storage;

public abstract class EFRepository<TAggregateRoot, TDbContext>(TDbContext context)
    : IRepository<TAggregateRoot>
    where TAggregateRoot : class, IAggregateRoot
    where TDbContext : DbContext, IUnitOfWork
{
    protected readonly TDbContext _context = context;
    protected DbSet<TAggregateRoot> Items => _context.Set<TAggregateRoot>();

    public IUnitOfWork UnitOfWork => _context;

    public ValueTask<TAggregateRoot?> FindAsync(
        object[] keys,
        CancellationToken cancellationToken
    ) => Items.FindAsync(keys, cancellationToken);

    public Task<TAggregateRoot?> FirstOrDefaultAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken
    ) => Items.FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<TAggregateRoot?> FirstOrDefaultAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        Expression<Func<TAggregateRoot, object>> orderBy,
        CancellationToken cancellationToken
    ) => Items.OrderBy(orderBy).FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<TAggregateRoot>> ListAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken
    ) => await Items.Where(predicate).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TAggregateRoot>> ListAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        Expression<Func<TAggregateRoot, object>> orderBy,
        CancellationToken cancellationToken
    ) => await Items.Where(predicate).OrderBy(orderBy).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TAggregateRoot>> ListAsync(
        Paginating paginating,
        Expression<Func<TAggregateRoot, object>> orderBy,
        CancellationToken cancellationToken
    ) =>
        await Items
            .OrderBy(orderBy)
            .Skip(paginating.PageIndex * paginating.PageSize)
            .Take(paginating.PageSize)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TAggregateRoot>> ListAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        Paginating paginating,
        Expression<Func<TAggregateRoot, object>> orderBy,
        CancellationToken cancellationToken
    ) =>
        await Items
            .Where(predicate)
            .OrderBy(orderBy)
            .Skip(paginating.PageIndex * paginating.PageSize)
            .Take(paginating.PageSize)
            .ToListAsync(cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken) =>
        Items.CountAsync(cancellationToken);

    public Task<int> CountAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken
    ) => Items.CountAsync(predicate, cancellationToken);

    public Task<int> CountQueryAsync(
        Func<IQueryable<TAggregateRoot>, IQueryable<TAggregateRoot>> predicate,
        CancellationToken cancellationToken
    ) => predicate(Items).CountAsync(cancellationToken);

    public async ValueTask<TAggregateRoot> AddAsync(
        TAggregateRoot aggregateRoot,
        CancellationToken cancellationToken
    ) => (await Items.AddAsync(aggregateRoot, cancellationToken)).Entity;

    public Task AddRangeAsync(
        IReadOnlyList<TAggregateRoot> aggregateRoots,
        CancellationToken cancellationToken
    ) => Items.AddRangeAsync(aggregateRoots, cancellationToken);

    public Task RemoveAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
    {
        Items.Remove(aggregateRoot);
        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(
        IReadOnlyList<TAggregateRoot> aggregateRoots,
        CancellationToken cancellationToken
    )
    {
        Items.RemoveRange(aggregateRoots);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<TResult>> QueryAsync<TResult>(
        Func<IQueryable<TAggregateRoot>, IQueryable<TResult>> predicate,
        CancellationToken cancellationToken
    ) => await predicate(Items).ToListAsync(cancellationToken);
}
