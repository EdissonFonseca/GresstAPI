using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class InventoryEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var inventory = group.MapGroup("/inventory")
            .WithTags("Inventory");

        inventory.MapGet("", async (
                [FromQuery] string? personId,
                [FromQuery] string? facilityId,
                [FromQuery] string? locationId,
                [FromQuery] string? wasteTypeId,
                IBalanceService balanceService,
                CancellationToken ct) =>
            {
                var query = new InventoryQueryDto
                {
                    PersonId = personId,
                    FacilityId = facilityId,
                    LocationId = locationId,
                    WasteClassId = wasteTypeId
                };
                var list = await balanceService.GetInventoryAsync(query, ct);
                return Results.Ok(list);
            })
            .WithName("GetInventory");

        inventory.MapGet("balance", async (
                [FromQuery] string? personId,
                [FromQuery] string? facilityId,
                [FromQuery] string? locationId,
                [FromQuery] string wasteTypeId,
                IBalanceService balanceService,
                CancellationToken ct) =>
            {
                var balance = await balanceService.GetBalanceAsync(personId, facilityId, locationId, wasteTypeId, ct);
                if (balance == null)
                    return Results.NotFound("Balance not found");
                return Results.Ok(balance);
            })
            .WithName("GetBalance");

        return group;
    }
}
