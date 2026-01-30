using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class ServiceEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var services = group.MapGroup("/services")
            .WithTags("Service");

        services.MapGet("types", async (IServiceService serviceService, CancellationToken ct) =>
            {
                var list = await serviceService.GetAllServicesAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllServices");

        services.MapGet("types/{id}", async (string id, IServiceService serviceService, CancellationToken ct) =>
            {
                var service = await serviceService.GetServiceByIdAsync(id, ct);
                if (service == null)
                    return Results.NotFound(new { message = "Service not found" });
                return Results.Ok(service);
            })
            .WithName("GetServiceById");

        services.MapPost("types", async ([FromBody] CreateServiceDto dto, IServiceService serviceService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var service = await serviceService.CreateServiceAsync(dto, ct);
                return Results.CreatedAtRoute("GetServiceById", new { id = service.Id }, service);
            })
            .WithName("CreateService");

        services.MapPut("types/{id}", async (string id, [FromBody] UpdateServiceDto dto, IServiceService serviceService, CancellationToken ct) =>
            {
                if (id.ToString() != dto.Id)
                    return Results.BadRequest(new { message = "ID mismatch" });
                if (dto == null)
                    return Results.BadRequest();
                var service = await serviceService.UpdateServiceAsync(dto, ct);
                if (service == null)
                    return Results.NotFound(new { message = "Service not found" });
                return Results.Ok(service);
            })
            .WithName("UpdateService");

        services.MapDelete("types/{id}", async (string id, IServiceService serviceService, CancellationToken ct) =>
            {
                var success = await serviceService.DeleteServiceAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { message = "Service not found" });
                return Results.NoContent();
            })
            .WithName("DeleteService");

        services.MapGet("account", async (IServiceService serviceService, CancellationToken ct) =>
            {
                var list = await serviceService.GetAccountPersonServicesAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAccountPersonServices");

        services.MapPost("account", async ([FromBody] CreatePersonServiceDto dto, IServiceService serviceService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var personService = await serviceService.CreateAccountPersonServiceAsync(dto, ct);
                return Results.Created("/api/v1/services/account", personService);
            })
            .WithName("CreateAccountPersonService");

        services.MapPut("account", async ([FromBody] UpdatePersonServiceDto dto, IServiceService serviceService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var personService = await serviceService.UpdateAccountPersonServiceAsync(dto, ct);
                if (personService == null)
                    return Results.NotFound(new { message = "PersonService not found" });
                return Results.Ok(personService);
            })
            .WithName("UpdateAccountPersonService");

        services.MapDelete("account/{serviceId}/{startDate:datetime}", async (string serviceId, DateTime startDate, IServiceService serviceService, CancellationToken ct) =>
            {
                var success = await serviceService.DeleteAccountPersonServiceAsync(serviceId, startDate, ct);
                if (!success)
                    return Results.NotFound(new { message = "PersonService not found" });
                return Results.NoContent();
            })
            .WithName("DeleteAccountPersonService");

        return group;
    }
}
