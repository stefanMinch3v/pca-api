using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pca.Domain.Contacts;
using pca.Domain.Contacts.ValueObjects;

namespace pca.Infrastructure.Persistence.Configurations;

/// <summary>
/// Maps the <see cref="Contact"/> aggregate straight onto the "Contacts"
/// table - no separate persistence model. EF Core materializes it (and the
/// owned <see cref="Address"/>) through their existing internal/private
/// constructors via reflection, so the rich domain model's encapsulation
/// (no public setters, no parameterless constructors) doesn't need to be
/// weakened for persistence.
/// </summary>
public sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.FirstName)
            .HasMaxLength(Contact.MaxNameLength)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasMaxLength(Contact.MaxNameLength)
            .IsRequired();

        builder.Property(c => c.DateOfBirth)
            .IsRequired();

        // EF Core "complex type" (not an owned entity type / OwnsOne): stored
        // as plain columns on the Contacts table, with no identity or
        // navigation of its own. Unlike OwnsOne, complex properties support
        // constructor binding on the *owner* too, so Contact's constructor
        // (which takes an Address) can still be used as-is.
        builder.ComplexProperty(c => c.Address, address =>
        {
            address
                .Property(a => a.Street)
                .HasMaxLength(Address.MaxStreetLength)
                .IsRequired();
            address
                .Property(a => a.City)
                .HasMaxLength(Address.MaxCityLength)
                .IsRequired();
            address
                .Property(a => a.PostalCode)
                .HasMaxLength(Address.MaxPostalCodeLength)
                .IsRequired();
            address
                .Property(a => a.Country)
                .HasMaxLength(Address.MaxCountryLength)
                .IsRequired();
        });

        // PhoneNumber/Iban just wrap a single validated string, so a value
        // converter (scalar column) is a better fit than an owned type.
        builder.Property(c => c.PhoneNumber)
            .HasConversion(phoneNumber => phoneNumber.Value, value => PhoneNumber.Create(value))
            .HasMaxLength(PhoneNumber.MaxLength)
            .IsRequired();

        builder.Property(c => c.Iban)
            .HasConversion(iban => iban.Value, value => Iban.Create(value))
            .HasMaxLength(Iban.MaxLength)
            .IsRequired();
    }
}
