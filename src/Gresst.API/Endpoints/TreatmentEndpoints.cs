using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class TreatmentEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var treatments = group.MapGroup("/treatments")
            .WithTags("Treatment");

        treatments.MapGet("types", async (ITreatmentService treatmentService, CancellationToken ct) =>
            {
                var list = await treatmentService.GetAllTreatmentsAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllTreatments");

        treatments.MapGet("types/{id}", async (string id, ITreatmentService treatmentService, CancellationToken ct) =>
            {
                var treatment = await treatmentService.GetTreatmentByIdAsync(id, ct);
                if (treatment == null)
                    return Results.NotFound(new { message = "Treatment not found" });
                return Results.Ok(treatment);
            })
            .WithName("GetTreatmentById");

        treatments.MapPost("types", async ([FromBody] CreateTreatmentDto dto, ITreatmentService treatmentService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var treatment = await treatmentService.CreateTreatmentAsync(dto, ct);
                return Results.CreatedAtRoute("GetTreatmentById", new { id = treatment.Id }, treatment);
            })
            .WithName("CreateTreatment");

        treatments.MapPut("types/{id}", async (string id, [FromBody] UpdateTreatmentDto dto, ITreatmentService treatmentService, CancellationToken ct) =>
            {
                if (id != dto.Id)
                    return Results.BadRequest(new { message = "ID mismatch" });
                if (dto == null)
                    return Results.BadRequest();
                var treatment = await treatmentService.UpdateTreatmentAsync(dto, ct);
                if (treatment == null)
                    return Results.NotFound(new { message = "Treatment not found" });
                return Results.Ok(treatment);
            })
            .WithName("UpdateTreatment");

        treatments.MapDelete("types/{id}", async (string id, ITreatmentService treatmentService, CancellationToken ct) =>
            {
                var success = await treatmentService.DeleteTreatmentAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { message = "Treatment not found" });
                return Results.NoContent();
            })
            .WithName("DeleteTreatment");

        treatments.MapGet("account", async (ITreatmentService treatmentService, CancellationToken ct) =>
            {
                var list = await treatmentService.GetAccountPersonTreatmentsAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAccountPersonTreatments");

        treatments.MapPost("account", async ([FromBody] CreatePersonTreatmentDto dto, ITreatmentService treatmentService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var personTreatment = await treatmentService.CreateAccountPersonTreatmentAsync(dto, ct);
                return Results.Created("/api/v1/treatments/account", personTreatment);
            })
            .WithName("CreateAccountPersonTreatment");

        treatments.MapPut("account", async ([FromBody] UpdatePersonTreatmentDto dto, ITreatmentService treatmentService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var personTreatment = await treatmentService.UpdateAccountPersonTreatmentAsync(dto, ct);
                if (personTreatment == null)
                    return Results.NotFound(new { message = "PersonTreatment not found" });
                return Results.Ok(personTreatment);
            })
            .WithName("UpdateAccountPersonTreatment");

        treatments.MapDelete("account/{treatmentId}", async (string treatmentId, ITreatmentService treatmentService, CancellationToken ct) =>
            {
                var success = await treatmentService.DeleteAccountPersonTreatmentAsync(treatmentId, ct);
                if (!success)
                    return Results.NotFound(new { message = "PersonTreatment not found" });
                return Results.NoContent();
            })
            .WithName("DeleteAccountPersonTreatment");

        return group;
    }
}
