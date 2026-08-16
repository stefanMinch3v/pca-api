namespace pca.Application.Common;

/// <summary>
/// A page of items returned via keyset (page-key) pagination
/// </summary>
public sealed record Page<TItem>(IReadOnlyList<TItem> Items, string? NextPageKey)
{
    public bool HasMore => NextPageKey is not null;
}
