using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class MaterialEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var materials = group.MapGroup("/materials")
            .WithTags("Material");

        materials.MapGet("", async (IMaterialService materialService, CancellationToken ct) =>
            {
                var list = await materialService.GetAccountPersonMaterialsAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllMaterials");

        materials.MapGet("account", async (IMaterialService materialService, CancellationToken ct) =>
            {
                var list = await materialService.GetAccountPersonMaterialsAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAccountPersonMaterials");

        materials.MapGet("{id}", async (string id, IMaterialService materialService, CancellationToken ct) =>
            {
                var material = await materialService.GetByIdAsync(id.ToString(), ct);
                if (material == null)
                    return Results.NotFound(new { message = "Material not found or you don't have access" });
                return Results.Ok(material);
            })
            .WithName("GetMaterialById");

        materials.MapGet("wastetype/{wasteTypeId}", async (string wasteTypeId, IMaterialService materialService, CancellationToken ct) =>
            {
                var list = await materialService.GetByWasteClassAsync(wasteTypeId, ct);
                return Results.Ok(list);
            })
            .WithName("GetMaterialsByWasteClass");

        materials.MapGet("category/{category}", async (string category, IMaterialService materialService, CancellationToken ct) =>
            {
                var list = await materialService.GetByCategoryAsync(category, ct);
                return Results.Ok(list);
            })
            .WithName("GetMaterialsByCategory");

        materials.MapPost("", async ([FromBody] CreateMaterialDto dto, IMaterialService materialService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var material = await materialService.CreateAccountPersonMaterialAsync(dto, ct);
                return Results.CreatedAtRoute("GetMaterialById", new { id = material.Id }, material);
            })
            .WithName("CreateMaterial");

        materials.MapPost("account", async ([FromBody] CreateMaterialDto dto, IMaterialService materialService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var material = await materialService.CreateAccountPersonMaterialAsync(dto, ct);
                return Results.CreatedAtRoute("GetMaterialById", new { id = material.Id }, material);
            })
            .WithName("CreateAccountPersonMaterial");

        materials.MapPut("{id}", async (string id, [FromBody] UpdateMaterialDto dto, IMaterialService materialService, CancellationToken ct) =>
            {
                if (id != dto.Id)
                    return Results.BadRequest(new { message = "ID mismatch" });
                if (dto == null)
                    return Results.BadRequest();
                var material = await materialService.UpdateAsync(dto, ct);
                if (material == null)
                    return Results.NotFound(new { message = "Material not found or you don't have access" });
                return Results.Ok(material);
            })
            .WithName("UpdateMaterial");

        materials.MapDelete("{id}", async (string id, IMaterialService materialService, CancellationToken ct) =>
            {
                var success = await materialService.DeleteAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { message = "Material not found or you don't have access" });
                return Results.NoContent();
            })
            .WithName("DeleteMaterial");

        return group;
    }
}
