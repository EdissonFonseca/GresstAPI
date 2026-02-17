using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

/// <summary>
/// Collections: order-centric views (Orden = internal planning/execution). Tasks assigned to a driver, planned for a date.
/// Uses request (Solicitud) data enriched with order/planning. See docs/requests-and-orders.md.
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
                var personId = currentUser.GetCurrentAccountPersonId();
                if (string.IsNullOrEmpty(personId))
                    return Results.Unauthorized();
                var filterDate = date ?? DateTime.Today;
                var list = await processService.GetCollectionsForDriverAsync(personId, filterDate, ct);
                return Results.Ok(new { Processes = list });
            })
            .WithName("GetCollections")
            .WithSummary("Get collection tasks for the current driver. Optional query: date=yyyy-MM-dd (default: today).");

        // GET /api/v1/collections/pending?date=yyyy-MM-dd&driverId=...
        collections.MapGet("pending", async (
                [FromQuery] DateTime? date,
                [FromQuery] string? driverId,
                IProcessService processService,
                ICurrentUserService currentUser,
                CancellationToken ct) =>
            {
                var accountPersonId = currentUser.GetCurrentAccountPersonId();
                if (string.IsNullOrEmpty(accountPersonId))
                    return Results.Unauthorized();
                var items = await processService.GetPendientesRecoleccionWithPlanningAsync(
                    accountPersonId,
                    date,
                    driverId,
                    idServicio: null,
                    ct);
                return Results.Ok(new { Items = items });
            })
            .WithName("GetPendingCollectionItems")
            .WithSummary("Get items pending collection (with planning). Optional: date=yyyy-MM-dd (planned date), driverId (assigned driver/conductor).");

        return group;
    }
}
