using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class VehicleEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var vehicles = group.MapGroup("/vehicle")
            .WithTags("Vehicle");

        vehicles.MapGet("", async (IVehicleService vehicleService, CancellationToken ct) =>
            {
                var list = await vehicleService.GetAccountPersonVehiclesAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllVehicles");

        vehicles.MapGet("account", async (IVehicleService vehicleService, CancellationToken ct) =>
            {
                var list = await vehicleService.GetAccountPersonVehiclesAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAccountPersonVehicles");

        vehicles.MapGet("{id}", async (string id, IVehicleService vehicleService, CancellationToken ct) =>
            {
                var vehicle = await vehicleService.GetByIdAsync(id, ct);
                if (vehicle == null)
                    return Results.NotFound(new { message = "Vehicle not found or you don't have access" });
                return Results.Ok(vehicle);
            })
            .WithName("GetVehicleById");

        vehicles.MapPost("", async ([FromBody] CreateVehicleDto dto, IVehicleService vehicleService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var vehicle = await vehicleService.CreateAccountPersonVehicleAsync(dto, ct);
                return Results.CreatedAtRoute("GetVehicleById", new { id = vehicle.Id }, vehicle);
            })
            .WithName("CreateVehicle");

        vehicles.MapPost("account", async ([FromBody] CreateVehicleDto dto, IVehicleService vehicleService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var vehicle = await vehicleService.CreateAccountPersonVehicleAsync(dto, ct);
                return Results.CreatedAtRoute("GetVehicleById", new { id = vehicle.Id }, vehicle);
            })
            .WithName("CreateAccountPersonVehicle");

        vehicles.MapPut("{id}", async (string id, [FromBody] UpdateVehicleDto dto, IVehicleService vehicleService, CancellationToken ct) =>
            {
                if (id != dto.Id)
                    return Results.BadRequest(new { message = "ID mismatch" });
                if (dto == null)
                    return Results.BadRequest();
                var vehicle = await vehicleService.UpdateAsync(dto, ct);
                if (vehicle == null)
                    return Results.NotFound(new { message = "Vehicle not found or you don't have access" });
                return Results.Ok(vehicle);
            })
            .WithName("UpdateVehicle");

        vehicles.MapDelete("{id}", async (string id, IVehicleService vehicleService, CancellationToken ct) =>
            {
                var success = await vehicleService.DeleteAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { message = "Vehicle not found or you don't have access" });
                return Results.NoContent();
            })
            .WithName("DeleteVehicle");

        return group;
    }
}
