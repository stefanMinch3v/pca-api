using MediatR;
using Microsoft.EntityFrameworkCore;
using pca.Application.Common;
using pca.Application.Common.Interfaces;
using pca.Application.Contacts.OutputModels;

namespace pca.Application.Contacts.Queries.GetAllContacts;

/// <summary>
/// Lists contacts using keyset (page-key) pagination ordered by <c>Id</c>
/// descending - not offset/skip-take. Ids are UUIDv7, whose leading bits are
/// a millisecond timestamp, so "ordered by Id desc" already means "newest
/// first" and "Id &lt; last seen id" already means "older than the last
/// item seen". That makes the id a stable, efficient basis for the page
/// key: unlike offset paging, results don't shift around when rows are
/// inserted/deleted between page requests, and the database can seek on the
/// primary key instead of scanning/counting past skipped rows.
/// </summary>
public class GetAllContactsQuery : IRequest<Result<Page<ContactListingOutputModel>>>
{
    public const int DefaultPageSize = 10;

    /// <summary>
    /// Opaque, encoded key (see <see cref="PageKeyEncoder"/>) from the
    /// previous page's <see cref="Page{TItem}.NextPageKey"/>.
    /// <see langword="null"/> to fetch the first page.
    /// </summary>
    public string? PageKey { get; set; }

    /// <summary>
    /// Payload encoded into <see cref="PageKey"/>. A record (not a bare
    /// Guid) so extra fields can be added later without breaking previously
    /// issued page keys - unknown/missing fields just deserialize as
    /// default.
    /// </summary>
    internal sealed record PageKeyPayload(Guid LastId);

    internal class GetAllContactsQueryHandler(IApplicationDbContext dbContext)
        : IRequestHandler<GetAllContactsQuery, Result<Page<ContactListingOutputModel>>>
    {
        public async Task<Result<Page<ContactListingOutputModel>>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
        {
            var query = dbContext.Contacts.AsNoTracking();

            var pageKey = PageKeyEncoder.Decode<PageKeyPayload>(request.PageKey);
            if (pageKey is not null)
            {
                query = query.Where(c => c.Id.CompareTo(pageKey.LastId) < 0);
            }

            // Fetch one extra row (never returned in the response) purely to
            // know whether a next page exists, without a separate count
            // query. The Select projects straight into the output model
            // (rather than materializing full Contact entities and mapping
            // in memory afterwards), so EF Core only selects the handful of
            // columns actually needed - skipping Address, Iban, DateOfBirth,
            // and the audit columns entirely.
            var items = await query
                .OrderByDescending(c => c.Id)
                .Take(DefaultPageSize + 1)
                .Select(contact => new ContactListingOutputModel(
                    contact.Id,
                    contact.FirstName,
                    contact.LastName,
                    contact.PhoneNumber.Value))
                .ToListAsync(cancellationToken);

            var hasMore = items.Count > DefaultPageSize;
            if (hasMore)
            {
                items.RemoveAt(items.Count - 1);
            }

            var nextPageKey = hasMore
                ? PageKeyEncoder.Encode(new PageKeyPayload(items[^1].Id))
                : null;

            return Result<Page<ContactListingOutputModel>>.SuccessWith(new Page<ContactListingOutputModel>(items, nextPageKey));
        }
    }
}
