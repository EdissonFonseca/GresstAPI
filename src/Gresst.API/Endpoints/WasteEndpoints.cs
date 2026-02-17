using Gresst.API;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class WasteEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var wastes = group.MapGroup("/wastes")
            .WithTags("Waste");

        wastes.MapGet("", async (IWasteService wasteService, CancellationToken ct) =>
            {
                var list = await wasteService.GetAllAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllWastes");

        wastes.MapGet("{id}", async (string id, IWasteService wasteService, CancellationToken ct) =>
            {
                try
                {
                    var waste = await wasteService.GetByIdAsync(id, ct);
                    return Results.Ok(waste);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("GetWasteById");

        wastes.MapGet("generator/{generatorId}", async (string generatorId, IWasteService wasteService, CancellationToken ct) =>
            {
                var list = await wasteService.GetByGeneratorAsync(generatorId, ct);
                return Results.Ok(list);
            })
            .WithName("GetWastesByGenerator");

        wastes.MapGet("status/{status}", async (string status, IWasteService wasteService, CancellationToken ct) =>
            {
                try
                {
                    var list = await wasteService.GetByStatusAsync(status, ct);
                    return Results.Ok(list);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("GetWastesByStatus");

        wastes.MapGet("bank", async (IWasteService wasteService, CancellationToken ct) =>
            {
                var list = await wasteService.GetWasteBankAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetWasteBank");

        wastes.MapPost("", async ([FromBody] CreateWasteDto dto, IWasteService wasteService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var waste = await wasteService.CreateAsync(dto, ct);
                return Results.CreatedAtRoute("GetWasteById", new { id = waste.Id }, waste);
            })
            .WithName("CreateWaste");

        // POST /wastes/register-generated — company generates waste at its facility → Residuo + Saldo (no Request). See docs/waste-generation.md.
        wastes.MapPost("register-generated", async ([FromBody] RegisterGeneratedWasteDto dto, IWasteGenerationService wasteGenerationService, CancellationToken ct) =>
            {
                if (dto == null || string.IsNullOrEmpty(dto.GeneratorPersonId) || dto.DepotId == 0 || dto.MaterialId == 0)
                    return Results.BadRequest(new { message = "GeneratorPersonId, DepotId, and MaterialId are required." });
                var result = await wasteGenerationService.RegisterGeneratedWasteAsync(dto, ct);
                return Results.Created($"/wastes/{result.IdResiduo}", result);
            })
            .WithName("RegisterGeneratedWaste")
            .WithSummary("Register waste generated at a company's facility. Creates Residuo and Saldo (inventory) directly; no Request.");

        wastes.MapPut("{id}", async (string id, [FromBody] UpdateWasteDto dto, IWasteService wasteService, CancellationToken ct) =>
            {
                if (id != dto.Id)
                    return Results.BadRequest("ID mismatch");
                try
                {
                    var waste = await wasteService.UpdateAsync(dto, ct);
                    return Results.Ok(waste);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("UpdateWaste");

        wastes.MapDelete("{id}", async (string id, IWasteService wasteService, CancellationToken ct) =>
            {
                try
                {
                    await wasteService.DeleteAsync(id, ct);
                    return Results.NoContent();
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("DeleteWaste");

        wastes.MapPost("{id}/publish-to-bank", async (string id, [FromBody] PublishToBankDto dto, IWasteService wasteService, CancellationToken ct) =>
            {
                try
                {
                    await wasteService.PublishToWasteBankAsync(id, dto.Description, dto.Price, ct);
                    return Results.Ok();
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("PublishToBank");

        wastes.MapPost("{id}/remove-from-bank", async (string id, IWasteService wasteService, CancellationToken ct) =>
            {
                try
                {
                    await wasteService.RemoveFromWasteBankAsync(id, ct);
                    return Results.Ok();
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("RemoveFromBank");

        return group;
    }
}
