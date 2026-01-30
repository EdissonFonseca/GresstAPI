using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class PersonContactEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var contacts = group.MapGroup("/personcontact")
            .WithTags("PersonContact")
            .RequireAuthorization();

        contacts.MapGet("account", async (IPersonContactService personContactService, ILogger<Program> logger, CancellationToken ct) =>
            {
                try
                {
                    var list = await personContactService.GetAccountPersonContactsAsync(ct);
                    return Results.Ok(list);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error getting account person contacts");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetAccountPersonContacts");

        contacts.MapGet("account/{contactId}", async (string contactId, [FromQuery] string? relationshipType, IPersonContactService personContactService, ILogger<Program> logger, CancellationToken ct) =>
            {
                try
                {
                    var contact = await personContactService.GetAccountPersonContactAsync(contactId, relationshipType, ct);
                    if (contact == null) return Results.NotFound();
                    return Results.Ok(contact);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error getting account person contact {ContactId}", contactId);
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetAccountPersonContact");

        contacts.MapPost("account", async ([FromBody] CreatePersonContactDto dto, IPersonContactService personContactService, ILogger<Program> logger, CancellationToken ct) =>
            {
                try
                {
                    if (dto == null) return Results.BadRequest();
                    var contact = await personContactService.CreateAccountPersonContactAsync(dto, ct);
                    return Results.Created($"/api/v1/personcontact/account/{contact.ContactId}", contact);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error creating account person contact");
                    return Results.StatusCode(500);
                }
            })
            .WithName("CreateAccountPersonContact");

        contacts.MapPut("account", async ([FromBody] UpdatePersonContactDto dto, IPersonContactService personContactService, ILogger<Program> logger, CancellationToken ct) =>
            {
                try
                {
                    if (dto == null) return Results.BadRequest();
                    var contact = await personContactService.UpdateAccountPersonContactAsync(dto, ct);
                    if (contact == null) return Results.NotFound();
                    return Results.Ok(contact);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error updating account person contact");
                    return Results.StatusCode(500);
                }
            })
            .WithName("UpdateAccountPersonContact");

        contacts.MapDelete("account/{contactId}", async (string contactId, [FromQuery] string? relationshipType, IPersonContactService personContactService, ILogger<Program> logger, CancellationToken ct) =>
            {
                try
                {
                    var deleted = await personContactService.DeleteAccountPersonContactAsync(contactId, relationshipType, ct);
                    if (!deleted) return Results.NotFound();
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error deleting account person contact {ContactId}", contactId);
                    return Results.StatusCode(500);
                }
            })
            .WithName("DeleteAccountPersonContact");

        return group;
    }
}
