using Microsoft.EntityFrameworkCore;
using pca.Domain.Contacts;

namespace pca.Application.Common.Interfaces;

/// <summary>
/// Persistence abstraction owned by the Application layer and implemented by
/// Infrastructure's <c>ApplicationDbContext</c> (DI).
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Contact> Contacts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
