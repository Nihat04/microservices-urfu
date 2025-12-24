namespace ProductService.Domain.Common;

public abstract class Entity<TId>(TId id)
{
    public TId Id { get; private set; } = id;

    protected bool Equals(Entity<TId> other) => EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((Entity<TId>)obj);
    }

    public override int GetHashCode() => Id?.GetHashCode() ?? 0;

    public override string ToString() => $"{GetType().Name}({nameof(Id)}: {Id})";
}
