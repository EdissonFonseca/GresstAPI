using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class PartyEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var parties = group.MapGroup("/parties")
            .WithTags("Party");

        parties.MapGet("", async (
            IPartyService partyService,
            CancellationToken ct) =>
        {
            var filter = new Application.Queries.PartyFilter(); // Sin rol específico para obtener todos
            var result = await partyService.GetAllAsync(ct);
            return Results.Ok(result);
        }).WithName("GetParties");

        parties.MapGet("/{partyId}", async (
            string partyId,
            IPartyService partyService,
            CancellationToken ct) =>
        {
            var result = await partyService.GetByIdAsync(partyId, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).WithName("GetPartyById");

        return group;
    }
}