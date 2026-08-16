using Microsoft.EntityFrameworkCore;
using pca.Application.Common.Interfaces;
using pca.Domain.Common;
using pca.Domain.Contacts;
using System.Reflection;

namespace pca.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        base.OnConfiguring(optionsBuilder);
    }

    public override int SaveChanges() => this.SaveChanges(true);

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.ApplyAuditInfoRules();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => this.SaveChangesAsync(true, cancellationToken);

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        this.ApplyAuditInfoRules();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditInfoRules()
    {
        var changedEntries = this.ChangeTracker
            .Entries()
            .Where(e => (e.Entity is IAuditable));

        foreach (var entry in changedEntries)
        {
            var entity = (IAuditable)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTimeOffset.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }
}
