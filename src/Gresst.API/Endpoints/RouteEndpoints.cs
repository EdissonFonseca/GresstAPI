using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class RouteEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var routes = group.MapGroup("/routes")
            .WithTags("Route");

        routes.MapGet("", async (IRouteService routeService, CancellationToken ct) =>
            {
                var list = await routeService.GetAllAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllRoutes");

        routes.MapGet("active", async (IRouteService routeService, CancellationToken ct) =>
            {
                var list = await routeService.GetActiveAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetActiveRoutes");

        routes.MapGet("type/{routeType}", async (string routeType, IRouteService routeService, CancellationToken ct) =>
            {
                var list = await routeService.GetByTypeAsync(routeType, ct);
                return Results.Ok(list);
            })
            .WithName("GetRoutesByType");

        routes.MapGet("vehicle/{vehicleId}", async (string vehicleId, IRouteService routeService, CancellationToken ct) =>
            {
                var list = await routeService.GetByVehicleAsync(vehicleId, ct);
                return Results.Ok(list);
            })
            .WithName("GetRoutesByVehicle");

        routes.MapGet("{id}", async (string id, IRouteService routeService, CancellationToken ct) =>
            {
                var route = await routeService.GetByIdAsync(id, ct);
                if (route == null)
                    return Results.NotFound(new { message = "Route not found" });
                return Results.Ok(route);
            })
            .WithName("GetRouteById");

        routes.MapPost("", async ([FromBody] CreateRouteDto dto, IRouteService routeService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var route = await routeService.CreateAsync(dto, ct);
                return Results.CreatedAtRoute("GetRouteById", new { id = route.Id }, route);
            })
            .WithName("CreateRoute");

        routes.MapPut("{id}", async (string id, [FromBody] UpdateRouteDto dto, IRouteService routeService, CancellationToken ct) =>
            {
                if (id != dto.Id)
                    return Results.BadRequest(new { message = "ID mismatch" });
                if (dto == null)
                    return Results.BadRequest();
                var route = await routeService.UpdateAsync(dto, ct);
                if (route == null)
                    return Results.NotFound(new { message = "Route not found" });
                return Results.Ok(route);
            })
            .WithName("UpdateRoute");

        routes.MapDelete("{id}", async (string id, IRouteService routeService, CancellationToken ct) =>
            {
                var success = await routeService.DeactivateAsync(id, ct);
                if (success == null)
                    return Results.NotFound(new { message = "Route not found" });
                return Results.NoContent();
            })
            .WithName("DeleteRoute");

        routes.MapPost("{routeId}/stops", async (string routeId, [FromBody] CreateRouteStopDto dto, IRouteService routeService, CancellationToken ct) =>
            {
                if (dto == null || string.IsNullOrEmpty(dto.FacilityId))
                    return Results.BadRequest(new { message = "FacilityId is required for RouteStop" });
                var stop = await routeService.AddStopAsync(routeId, dto, ct);
                return Results.Created($"/api/v1/routes/{routeId}", stop);
            })
            .WithName("AddRouteStop");

        routes.MapDelete("{routeId}/stops/{facilityId}", async (string routeId, string facilityId, IRouteService routeService, CancellationToken ct) =>
            {
                var success = await routeService.RemoveStopAsync(routeId, facilityId, ct);
                if (!success)
                    return Results.NotFound(new { message = "RouteStop not found" });
                return Results.NoContent();
            })
            .WithName("RemoveRouteStop");

        routes.MapPut("{routeId}/stops/reorder", async (string routeId, [FromBody] Dictionary<string, int> stopSequences, IRouteService routeService, CancellationToken ct) =>
            {
                if (stopSequences == null)
                    return Results.BadRequest();
                var route = await routeService.ReorderStopsAsync(routeId, stopSequences, ct);
                if (route == null)
                    return Results.NotFound(new { message = "Route not found" });
                return Results.Ok(route);
            })
            .WithName("ReorderRouteStops");

        routes.MapPost("{id}/activate", async (string id, IRouteService routeService, CancellationToken ct) =>
            {
                var route = await routeService.ActivateAsync(id, ct);
                if (route == null)
                    return Results.NotFound(new { message = "Route not found" });
                return Results.Ok(route);
            })
            .WithName("ActivateRoute");

        routes.MapPost("{id}/deactivate", async (string id, IRouteService routeService, CancellationToken ct) =>
            {
                var route = await routeService.DeactivateAsync(id, ct);
                if (route == null)
                    return Results.NotFound(new { message = "Route not found" });
                return Results.Ok(route);
            })
            .WithName("DeactivateRoute");

        return group;
    }
}
