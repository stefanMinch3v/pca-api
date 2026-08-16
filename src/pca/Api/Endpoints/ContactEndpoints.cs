using MediatR;
using pca.Api.Extensions;
using pca.Application.Contacts.Commands.CreateContact;
using pca.Application.Contacts.Commands.DeleteContact;
using pca.Application.Contacts.Commands.UpdateContact;
using pca.Application.Contacts.InputModels;
using pca.Application.Contacts.Queries.GetAllContacts;
using pca.Application.Contacts.Queries.GetContactById;

namespace pca.Api.Endpoints;

public static class ContactEndpoints
{
    public static IEndpointRouteBuilder MapContactEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/contacts").WithTags("Contacts");

        group.MapPost("/", CreateContact);
        group.MapGet("/{id:guid}", GetContactById);
        group.MapGet("/", GetAllContacts);
        group.MapPut("/{id:guid}", UpdateContact);
        group.MapDelete("/{id:guid}", DeleteContact);

        return app;
    }

    private static async Task<IResult> CreateContact(
        ISender sender,
        ContactInputModel contact,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateContactCommand { Contact = contact }, cancellationToken);

        return result.Succeeded
            ? Results.Created($"/api/contacts/{result.Value.Id}", result.Value)
            : result.ToProblemDetails();
    }

    private static async Task<IResult> GetContactById(
        ISender sender,
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContactByIdQuery { Id = id }, cancellationToken);

        // GetById's only failure mode is "no such contact" - no separate
        // validation branch to distinguish, so a failure just means 404.
        return result.Succeeded ? Results.Ok(result.Value) : Results.NotFound();
    }

    private static async Task<IResult> GetAllContacts(
        ISender sender,
        string? pageKey,
        CancellationToken cancellationToken)
    {
        var query = new GetAllContactsQuery { PageKey = pageKey };

        var result = await sender.Send(query, cancellationToken);

        // A failure here is a genuine PageKey validation problem (there's
        // no single entity to "not find"), so it goes through the regular
        // ProblemDetails path.
        return result.Succeeded ? Results.Ok(result.Value) : result.ToProblemDetails();
    }

    private static async Task<IResult> UpdateContact(
        ISender sender,
        Guid id,
        ContactInputModel contact,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateContactCommand { Id = id, Contact = contact }, cancellationToken);

        return result.Succeeded ? Results.Ok(result.Value) : result.ToProblemDetails();
    }

    private static async Task<IResult> DeleteContact(
        ISender sender,
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteContactCommand { Id = id }, cancellationToken);

        return result.Succeeded ? Results.NoContent() : result.ToProblemDetails();
    }
}
