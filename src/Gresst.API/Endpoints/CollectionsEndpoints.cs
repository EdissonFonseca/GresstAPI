using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

/// <summary>
/// Mobile app "Recolecciones" (collections): tasks assigned to the current driver.
/// Equivalent to legacy GetTransaction / fnResiduosPendientesRecoleccion.
/// </summary>
public static class CollectionsEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var collections = group.MapGroup("/collections")
            .WithTags("Collections")
            .RequireAuthorization();

        // GET /api/v1/collections or GET /api/v1/collections?date=yyyy-MM-dd
        collections.MapGet("", async (
                [FromQuery] DateTime? date,
                IProcessService processService,
                ICurrentUserService currentUser,
                CancellationToken ct) =>
            {
                var personId = currentUser.GetCurrentPersonId();
                if (string.IsNullOrEmpty(personId))
                    return Results.Unauthorized();
                var filterDate = date ?? DateTime.Today;
                var list = await processService.GetCollectionsForDriverAsync(personId, filterDate, ct);
                return Results.Ok(new { Processes = list });
            })
            .WithName("GetCollections")
            .WithSummary("Get collection tasks for the current driver. Optional query: date=yyyy-MM-dd (default: today).");

        return group;
    }
}
