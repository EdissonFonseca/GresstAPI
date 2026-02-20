using Gresst.Application.Services;
using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
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
            string? ownerId,
            string? role,
            bool? isActive,
            string? search,
            int? limit,
            string? next,
            CancellationToken ct) =>
        {
            PartyRelationType? roleEnum = null;
            if (!string.IsNullOrEmpty(role) && Enum.TryParse<PartyRelationType>(role, true, out var parsed))
                roleEnum = parsed;

            var take = Math.Clamp(limit ?? 50, 1, 200);
            var (items, nextCursor) = await partyService.FindPagedAsync(ownerId, roleEnum, isActive, search, take, next, ct);

            var response = new
            {
                Items = items,
                Next = nextCursor,
                Limit = take
            };

            return Results.Ok(response);
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