using pca.Domain.Common;
using pca.Domain.Contacts.ValueObjects;
using Xunit;

namespace pca.Domain.Contacts;

public class ContactBuilderSpecs
{
    private const string ValidIban = "GB29NWBK60161331926819";
    private const string ValidPhoneNumber = "+4479111234";
    private const string CityName = "London";
    private const string FirstName = "Bugs";
    private const string LastName = "Bunny";
    private static readonly DateOnly ValidDateOfBirth = new(1970, 5, 20);

    [Fact]
    public void Build_WithAllValidFields_ReturnsContactWithExpectedValues()
    {
        var contact = ValidBuilder().Build();

        Assert.NotEqual(Guid.Empty, contact.Id);
        Assert.Equal(7, contact.Id.Version);
        Assert.Equal(FirstName, contact.FirstName);
        Assert.Equal(LastName, contact.LastName);
        Assert.Equal(ValidDateOfBirth, contact.DateOfBirth);
        Assert.Equal(CityName, contact.Address.City);
        Assert.Equal(ValidPhoneNumber, contact.PhoneNumber.Value);
        Assert.Equal(ValidIban, contact.Iban.Value);
    }

    [Fact]
    public void Build_WithNoFieldsSet_ThrowsDomainValidationExceptionListingEveryMissingField()
    {
        var builder = new ContactBuilder();

        var exception = Assert.Throws<DomainValidationException>(() => builder.Build());

        Assert.Equal(6, exception.Errors.Count);
        Assert.Contains(Contact.FirstNameRequiredMessage, exception.Errors);
        Assert.Contains(Contact.LastNameRequiredMessage, exception.Errors);
        Assert.Contains(ContactBuilder.DateOfBirthRequiredMessage, exception.Errors);
        Assert.Contains(ContactBuilder.AddressRequiredMessage, exception.Errors);
        Assert.Contains(ContactBuilder.PhoneNumberRequiredMessage, exception.Errors);
        Assert.Contains(Iban.RequiredErrorMessage, exception.Errors);
    }

    [Fact]
    public void Build_WithBlankLastNameAndInvalidIban_CollectsBothErrorsTogether()
    {
        var builder = ValidBuilder()
            .WithLastName("   ")
            .WithIban("GB00INVALID000000000000");

        var exception = Assert.Throws<DomainValidationException>(() => builder.Build());

        Assert.Contains(Contact.LastNameRequiredMessage, exception.Errors);
        Assert.Contains(Iban.InvalidChecksumMessage, exception.Errors);
    }

    [Theory]
    [InlineData("gb29 nwbk 6016 1331 9268 19")]
    [InlineData("GB29NWBK60161331926819")]
    public void WithIban_NormalizesSpacingAndCasing(string rawIban)
    {
        var contact = ValidBuilder().WithIban(rawIban).Build();

        Assert.Equal(ValidIban, contact.Iban.Value);
    }

    [Fact]
    public void WithPhoneNumber_WithInvalidFormat_RecordsError()
    {
        var builder = ValidBuilder().WithPhoneNumber("not-a-number");

        var exception = Assert.Throws<DomainValidationException>(() => builder.Build());

        Assert.Contains(PhoneNumber.InvalidFormatMessage, exception.Errors);
    }

    [Fact]
    public void ApplyTo_WithValidFields_UpdatesExistingContactInPlace()
    {
        var contact = ValidBuilder().Build();
        var originalId = contact.Id;

        new ContactBuilder()
            .WithFirstName("Grace")
            .WithLastName("Hopper")
            .WithDateOfBirth(new DateOnly(1906, 12, 9))
            .WithAddress("2 Side St", "Manchester", "M1 1AE", "UK")
            .WithPhoneNumber("+447911999999")
            .WithIban("DE89370400440532013000")
            .ApplyTo(contact);

        Assert.Equal(originalId, contact.Id);
        Assert.Equal("Grace", contact.FirstName);
        Assert.Equal("Hopper", contact.LastName);
        Assert.Equal("Manchester", contact.Address.City);
        Assert.Equal("+447911999999", contact.PhoneNumber.Value);
        Assert.Equal("DE89370400440532013000", contact.Iban.Value);
    }

    [Fact]
    public void ApplyTo_WithMissingFields_ThrowsDomainValidationExceptionAndLeavesContactUnchanged()
    {
        var contact = ValidBuilder().Build();

        var exception = Assert.Throws<DomainValidationException>(() => new ContactBuilder().ApplyTo(contact));

        Assert.Equal(6, exception.Errors.Count);
        Assert.Equal(FirstName, contact.FirstName);
        Assert.Equal(ValidIban, contact.Iban.Value);
    }

    private static ContactBuilder ValidBuilder() =>
        new ContactBuilder()
            .WithFirstName(FirstName)
            .WithLastName(LastName)
            .WithDateOfBirth(ValidDateOfBirth)
            .WithAddress("1 Main St", CityName, "SW1A 1AA", "UK")
            .WithPhoneNumber(ValidPhoneNumber)
            .WithIban(ValidIban);
}
