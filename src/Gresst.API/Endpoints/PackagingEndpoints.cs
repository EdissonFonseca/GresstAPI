using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class PackagingEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var packagings = group.MapGroup("/packaging")
            .WithTags("Packaging");

        packagings.MapGet("", async (IPackagingService packagingService, CancellationToken ct) =>
            {
                var list = await packagingService.GetAllAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllPackagings");

        packagings.MapGet("{id}", async (string id, IPackagingService packagingService, CancellationToken ct) =>
            {
                var packaging = await packagingService.GetByIdAsync(id, ct);
                if (packaging == null)
                    return Results.NotFound(new { message = "Packaging not found" });
                return Results.Ok(packaging);
            })
            .WithName("GetPackagingById");

        packagings.MapGet("type/{packagingType}", async (string packagingType, IPackagingService packagingService, CancellationToken ct) =>
            {
                var list = await packagingService.GetByTypeAsync(packagingType, ct);
                return Results.Ok(list);
            })
            .WithName("GetPackagingsByType");

        packagings.MapPost("", async ([FromBody] CreatePackagingDto dto, IPackagingService packagingService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var packaging = await packagingService.CreateAsync(dto, ct);
                return Results.CreatedAtRoute("GetPackagingById", new { id = packaging.Id }, packaging);
            })
            .WithName("CreatePackaging");

        packagings.MapPut("{id}", async (string id, [FromBody] UpdatePackagingDto dto, IPackagingService packagingService, CancellationToken ct) =>
            {
                if (id != dto.Id)
                    return Results.BadRequest(new { message = "ID mismatch" });
                if (dto == null)
                    return Results.BadRequest();
                var packaging = await packagingService.UpdateAsync(dto, ct);
                if (packaging == null)
                    return Results.NotFound(new { message = "Packaging not found" });
                return Results.Ok(packaging);
            })
            .WithName("UpdatePackaging");

        packagings.MapDelete("{id}", async (string id, IPackagingService packagingService, CancellationToken ct) =>
            {
                var success = await packagingService.DeleteAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { message = "Packaging not found" });
                return Results.NoContent();
            })
            .WithName("DeletePackaging");

        return group;
    }
}
