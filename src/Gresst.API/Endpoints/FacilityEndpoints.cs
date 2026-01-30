using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class FacilityEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var facilities = group.MapGroup("/facility")
            .WithTags("Facility");

        facilities.MapGet("", async (IFacilityService facilityService, CancellationToken ct) =>
            {
                var list = await facilityService.GetAccountPersonFacilitiesAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllFacilities");

        facilities.MapGet("account", async (IFacilityService facilityService, CancellationToken ct) =>
            {
                var list = await facilityService.GetAccountPersonFacilitiesAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAccountPersonFacilities");

        facilities.MapGet("{id}", async (string id, IFacilityService facilityService, CancellationToken ct) =>
            {
                var facility = await facilityService.GetByIdAsync(id, ct);
                if (facility == null)
                    return Results.NotFound(new { message = "Facility not found or you don't have access" });
                return Results.Ok(facility);
            })
            .WithName("GetFacilityById");

        facilities.MapGet("person/{personId}", async (string personId, IFacilityService facilityService, CancellationToken ct) =>
            {
                var list = await facilityService.GetByPersonAsync(personId, ct);
                return Results.Ok(list);
            })
            .WithName("GetFacilitiesByPerson");

        facilities.MapGet("type/{type}", async (string type, IFacilityService facilityService, CancellationToken ct) =>
            {
                var list = await facilityService.GetByTypeAsync(type, ct);
                return Results.Ok(list);
            })
            .WithName("GetFacilitiesByType");

        facilities.MapPost("", async ([FromBody] CreateFacilityDto dto, IFacilityService facilityService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var facility = await facilityService.CreateAccountPersonFacilityAsync(dto, ct);
                return Results.CreatedAtRoute("GetFacilityById", new { id = facility.Id }, facility);
            })
            .WithName("CreateFacility");

        facilities.MapPost("account", async ([FromBody] CreateFacilityDto dto, IFacilityService facilityService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var facility = await facilityService.CreateAccountPersonFacilityAsync(dto, ct);
                return Results.CreatedAtRoute("GetFacilityById", new { id = facility.Id }, facility);
            })
            .WithName("CreateAccountPersonFacility");

        facilities.MapPut("{id}", async (string id, [FromBody] UpdateFacilityDto dto, IFacilityService facilityService, CancellationToken ct) =>
            {
                if (id != dto.Id)
                    return Results.BadRequest(new { message = "ID mismatch" });
                if (dto == null)
                    return Results.BadRequest();
                var facility = await facilityService.UpdateAsync(dto, ct);
                if (facility == null)
                    return Results.NotFound(new { message = "Facility not found or you don't have access" });
                return Results.Ok(facility);
            })
            .WithName("UpdateFacility");

        facilities.MapDelete("{id}", async (string id, IFacilityService facilityService, CancellationToken ct) =>
            {
                var success = await facilityService.DeleteAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { message = "Facility not found or you don't have access" });
                return Results.NoContent();
            })
            .WithName("DeleteFacility");

        facilities.MapGet("{facilityId}/material", async (string facilityId, IMaterialService materialService, CancellationToken ct) =>
            {
                var list = await materialService.GetFacilityMaterialsAsync(facilityId, ct);
                return Results.Ok(list);
            })
            .WithName("GetFacilityMaterials");

        facilities.MapPost("{facilityId}/material", async (string facilityId, [FromBody] CreateMaterialDto dto, IMaterialService materialService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var material = await materialService.CreateFacilityMaterialAsync(facilityId, dto, ct);
                return Results.Created($"/api/v1/facility/{facilityId}/material", material);
            })
            .WithName("CreateFacilityMaterial");

        return group;
    }
}
