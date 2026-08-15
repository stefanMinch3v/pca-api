using pca.Domain.Contacts;

namespace pca.Application.Contacts;

/// <summary>
/// Persistence abstraction for <see cref="Contact"/>, owned by the Application
/// layer and implemented by Infrastructure (Dependency Inversion) - replace with DbContext later
/// </summary>
public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Contact>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Contact contact, CancellationToken cancellationToken);

    Task UpdateAsync(Contact contact, CancellationToken cancellationToken);

    Task DeleteAsync(Contact contact, CancellationToken cancellationToken);
}
