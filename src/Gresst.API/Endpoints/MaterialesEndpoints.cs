using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class MaterialesEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var materiales = group.MapGroup("/materiales")
            .WithTags("Materiales")
            .RequireAuthorization();

        materiales.MapGet("get", async (
                [FromQuery] string? filter,
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                IMaterialService materialService,
                CancellationToken ct) =>
            {
                var materials = await materialService.GetAllAsync(ct);
                materials = materials.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                foreach (var material in materials)
                    material.Name = material.Name + "-" + material.Description + "-" + material.Measurement;
                return Results.Ok(materials);
            })
            .WithName("GetMateriales");

        return group;
    }
}
