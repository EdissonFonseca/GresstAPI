using Gresst.Application.Queries;
using Gresst.Application.Services;
using Gresst.Application.Services.Interfaces;
using Gresst.Domain.Entities;
using System.Linq.Expressions;

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
            bool? isActive,
            string? search,
            int? limit,
            string? next,
            CancellationToken ct) =>
        {
            Expression<Func<Party, bool>>? predicate = null;

            if (isActive.HasValue)
                predicate = (predicate ?? (p => true)).AndAlso(p => p.IsActive == isActive.Value);
            if (!string.IsNullOrEmpty(search))
            {
                var s = search.Trim();
                predicate = (predicate ?? (p => true)).AndAlso(p => p.Name != null && p.Name.Contains(s, StringComparison.OrdinalIgnoreCase));
            }
            var take = Math.Clamp(limit ?? 50, 1, 200);
            var (items, nextCursor) = await partyService.FindPagedAsync(predicate, ownerId, take, next, ct);

            return Results.Ok(new { Items = items, Next = nextCursor, Limit = take });
        }).WithName("GetParties");

        return group;
    }
}