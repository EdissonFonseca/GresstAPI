using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class WasteClassEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var wasteClasses = group.MapGroup("/wasteclasses")
            .WithTags("WasteClass");

        wasteClasses.MapGet("types", async (IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                var list = await wasteClassService.GetAllWasteClassesAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllWasteClasses");

        wasteClasses.MapGet("types/{id}", async (string id, IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                var wasteClass = await wasteClassService.GetWasteClassByIdAsync(id, ct);
                if (wasteClass == null)
                    return Results.NotFound(new { message = "WasteClass not found" });
                return Results.Ok(wasteClass);
            })
            .WithName("GetWasteClassById");

        wasteClasses.MapPost("types", async ([FromBody] CreateWasteClassDto dto, IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var wasteClass = await wasteClassService.CreateWasteClassAsync(dto, ct);
                return Results.CreatedAtRoute("GetWasteClassById", new { id = wasteClass.Id }, wasteClass);
            })
            .WithName("CreateWasteClass");

        wasteClasses.MapPut("types/{id}", async (string id, [FromBody] UpdateWasteClassDto dto, IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                if (id != dto.Id)
                    return Results.BadRequest(new { message = "ID mismatch" });
                if (dto == null)
                    return Results.BadRequest();
                var wasteClass = await wasteClassService.UpdateWasteClassAsync(dto, ct);
                if (wasteClass == null)
                    return Results.NotFound(new { message = "WasteClass not found" });
                return Results.Ok(wasteClass);
            })
            .WithName("UpdateWasteClass");

        wasteClasses.MapDelete("types/{id}", async (string id, IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                var success = await wasteClassService.DeleteWasteClassAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { message = "WasteClass not found" });
                return Results.NoContent();
            })
            .WithName("DeleteWasteClass");

        wasteClasses.MapGet("account", async (IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                var list = await wasteClassService.GetAccountPersonWasteClassesAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAccountPersonWasteClasses");

        wasteClasses.MapPost("account", async ([FromBody] CreatePersonWasteClassDto dto, IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var personWasteClass = await wasteClassService.CreateAccountPersonWasteClassAsync(dto, ct);
                return Results.Created("/api/v1/wasteclasses/account", personWasteClass);
            })
            .WithName("CreateAccountPersonWasteClass");

        wasteClasses.MapPut("account", async ([FromBody] UpdatePersonWasteClassDto dto, IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var personWasteClass = await wasteClassService.UpdateAccountPersonWasteClassAsync(dto, ct);
                if (personWasteClass == null)
                    return Results.NotFound(new { message = "PersonWasteClass not found" });
                return Results.Ok(personWasteClass);
            })
            .WithName("UpdateAccountPersonWasteClass");

        wasteClasses.MapDelete("account/{wasteClassId}", async (string wasteClassId, IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                var success = await wasteClassService.DeleteAccountPersonWasteClassAsync(wasteClassId, ct);
                if (!success)
                    return Results.NotFound(new { message = "PersonWasteClass not found" });
                return Results.NoContent();
            })
            .WithName("DeleteAccountPersonWasteClass");

        return group;
    }
}
