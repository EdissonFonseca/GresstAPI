using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class SupplyEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var supplies = group.MapGroup("/supplies")
            .WithTags("Supply");

        supplies.MapGet("", async (ISupplyService supplyService, CancellationToken ct) =>
            {
                var list = await supplyService.GetAllAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllSupplies");

        supplies.MapGet("public", async (ISupplyService supplyService, CancellationToken ct) =>
            {
                var list = await supplyService.GetPublicSuppliesAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetPublicSupplies");

        supplies.MapGet("category/{categoryUnitId}", async (string categoryUnitId, ISupplyService supplyService, CancellationToken ct) =>
            {
                var list = await supplyService.GetByCategoryAsync(categoryUnitId, ct);
                return Results.Ok(list);
            })
            .WithName("GetSuppliesByCategory");

        supplies.MapGet("{id}", async (string id, ISupplyService supplyService, CancellationToken ct) =>
            {
                var supply = await supplyService.GetByIdAsync(id, ct);
                if (supply == null)
                    return Results.NotFound(new { message = "Supply not found" });
                return Results.Ok(supply);
            })
            .WithName("GetSupplyById");

        supplies.MapPost("", async ([FromBody] CreateSupplyDto dto, ISupplyService supplyService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var supply = await supplyService.CreateAsync(dto, ct);
                return Results.CreatedAtRoute("GetSupplyById", new { id = supply.Id }, supply);
            })
            .WithName("CreateSupply");

        supplies.MapPut("{id}", async (string id, [FromBody] UpdateSupplyDto dto, ISupplyService supplyService, CancellationToken ct) =>
            {
                if (id != dto.Id)
                    return Results.BadRequest(new { message = "ID mismatch" });
                if (dto == null)
                    return Results.BadRequest();
                var supply = await supplyService.UpdateAsync(dto, ct);
                if (supply == null)
                    return Results.NotFound(new { message = "Supply not found" });
                return Results.Ok(supply);
            })
            .WithName("UpdateSupply");

        supplies.MapDelete("{id}", async (string id, ISupplyService supplyService, CancellationToken ct) =>
            {
                var success = await supplyService.DeleteAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { message = "Supply not found" });
                return Results.NoContent();
            })
            .WithName("DeleteSupply");

        return group;
    }
}
