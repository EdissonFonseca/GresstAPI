using Gresst.API;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class ManagementEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var management = group.MapGroup("/management")
            .WithTags("Management");

        management.MapGet("waste/{wasteId}/history", async (string wasteId, IManagementService managementService, CancellationToken ct) =>
            {
                var history = await managementService.GetWasteHistoryAsync(wasteId, ct);
                return Results.Ok(history);
            })
            .WithName("GetWasteHistory");

        management.MapPost("", async ([FromBody] CreateManagementDto dto, IManagementService managementService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var managementDto = await managementService.CreateManagementAsync(dto, ct);
                return Results.Created($"/api/v1/management/waste/{dto.WasteId}/history", managementDto);
            })
            .WithName("CreateManagement");

        management.MapPost("generate", async ([FromBody] CreateWasteDto dto, IManagementService managementService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var managementDto = await managementService.GenerateWasteAsync(dto, ct);
                return Results.Ok(managementDto);
            })
            .WithName("GenerateWaste");

        management.MapPost("collect", async ([FromBody] CollectWasteDto dto, IManagementService managementService, CancellationToken ct) =>
            {
                try
                {
                    if (dto == null)
                        return Results.BadRequest();
                    var managementDto = await managementService.CollectWasteAsync(dto, ct);
                    return Results.Ok(managementDto);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("CollectWaste");

        management.MapPost("transport", async ([FromBody] TransportWasteDto dto, IManagementService managementService, CancellationToken ct) =>
            {
                try
                {
                    if (dto == null)
                        return Results.BadRequest();
                    var managementDto = await managementService.TransportWasteAsync(dto, ct);
                    return Results.Ok(managementDto);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("TransportWaste");

        management.MapPost("receive", async ([FromBody] ReceiveWasteDto dto, IManagementService managementService, CancellationToken ct) =>
            {
                try
                {
                    if (dto == null)
                        return Results.BadRequest();
                    var managementDto = await managementService.ReceiveWasteAsync(dto.WasteId, dto.ReceiverId, dto.FacilityId, ct);
                    return Results.Ok(managementDto);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("ReceiveWaste");

        management.MapPost("transform", async ([FromBody] TransformWasteDto dto, IManagementService managementService, CancellationToken ct) =>
            {
                try
                {
                    if (dto == null)
                        return Results.BadRequest();
                    var managementDto = await managementService.TransformWasteAsync(dto, ct);
                    return Results.Ok(managementDto);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("TransformWaste");

        management.MapPost("store", async ([FromBody] StoreWasteDto dto, IManagementService managementService, CancellationToken ct) =>
            {
                try
                {
                    if (dto == null)
                        return Results.BadRequest();
                    var managementDto = await managementService.StoreWasteAsync(dto, ct);
                    return Results.Ok(managementDto);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("StoreWaste");

        management.MapPost("sell", async ([FromBody] SellWasteDto dto, IManagementService managementService, CancellationToken ct) =>
            {
                try
                {
                    if (dto == null)
                        return Results.BadRequest();
                    var managementDto = await managementService.SellWasteAsync(dto.WasteId, dto.BuyerId, dto.Price, ct);
                    return Results.Ok(managementDto);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("SellWaste");

        management.MapPost("deliver", async ([FromBody] DeliverWasteDto dto, IManagementService managementService, CancellationToken ct) =>
            {
                try
                {
                    if (dto == null)
                        return Results.BadRequest();
                    var managementDto = await managementService.DeliverToThirdPartyAsync(dto.WasteId, dto.RecipientId, dto.Notes ?? "", ct);
                    return Results.Ok(managementDto);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("DeliverWaste");

        management.MapPost("dispose", async ([FromBody] DisposeWasteDto dto, IManagementService managementService, CancellationToken ct) =>
            {
                try
                {
                    if (dto == null)
                        return Results.BadRequest();
                    var managementDto = await managementService.DisposeWasteAsync(dto, ct);
                    return Results.Ok(managementDto);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("DisposeWaste");

        management.MapPost("classify", async ([FromBody] ClassifyWasteDto dto, IManagementService managementService, CancellationToken ct) =>
            {
                try
                {
                    if (dto == null)
                        return Results.BadRequest();
                    var managementDto = await managementService.ClassifyWasteAsync(dto.WasteId, dto.WasteClassId, dto.ClassifiedById, ct);
                    return Results.Ok(managementDto);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
            })
            .WithName("ClassifyWaste");

        return group;
    }
}
