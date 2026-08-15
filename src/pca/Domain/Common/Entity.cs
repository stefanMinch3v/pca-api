namespace pca.Domain.Common;

/// <summary>
/// Base class for domain entities. Identity (not structural) equality is used,
/// scoped to entities of the exact same runtime type.
/// Used guid for simplicity, can be used as Entity<TId> where TId : struct
/// </summary>
public abstract class Entity
{
    public Guid Id { get; }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty || id.Version != 7)
        {
            throw new ArgumentException("Entity id must be a non-empty UUID v7.", nameof(id));
        }

        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other || other.GetType() != GetType())
        {
            return false;
        }

        return Id == other.Id;
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right) => !(left == right);
}
