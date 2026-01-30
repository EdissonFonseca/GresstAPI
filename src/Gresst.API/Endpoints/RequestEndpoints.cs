using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class RequestEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var requests = group.MapGroup("/requests")
            .WithTags("Request")
            .RequireAuthorization();

        requests.MapGet("mobile-transport-waste", async (IRequestService? requestService, ICurrentUserService currentUserService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                var personId = currentUserService.GetCurrentPersonId();
                if (string.IsNullOrEmpty(personId))
                    return Results.BadRequest(new { message = "Person ID not found for current user" });
                var list = await requestService.GetMobileTransportWasteAsync(personId, ct);
                return Results.Ok(list);
            })
            .WithName("GetMobileTransportWaste");

        requests.MapGet("mobile-transport-waste/{personId}", async (string personId, IRequestService? requestService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                if (string.IsNullOrEmpty(personId))
                    return Results.BadRequest(new { message = "Invalid person ID" });
                var list = await requestService.GetMobileTransportWasteAsync(personId, ct);
                return Results.Ok(list);
            })
            .WithName("GetMobileTransportWasteByPersonId");

        requests.MapGet("", async (IRequestService? requestService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                var list = await requestService.GetAllAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllRequests");

        requests.MapGet("{id}", async (string id, IRequestService? requestService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                var request = await requestService.GetByIdAsync(id, ct);
                if (request == null)
                    return Results.NotFound(new { message = "Request not found or you don't have access" });
                return Results.Ok(request);
            })
            .WithName("GetRequestById");

        requests.MapGet("requester/{requesterId}", async (string requesterId, IRequestService? requestService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                var list = await requestService.GetByRequesterAsync(requesterId, ct);
                return Results.Ok(list);
            })
            .WithName("GetRequestsByRequester");

        requests.MapGet("provider/{providerId}", async (string providerId, IRequestService? requestService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                var list = await requestService.GetByProviderAsync(providerId, ct);
                return Results.Ok(list);
            })
            .WithName("GetRequestsByProvider");

        requests.MapGet("status/{status}", async (string status, IRequestService? requestService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                var list = await requestService.GetByStatusAsync(status, ct);
                return Results.Ok(list);
            })
            .WithName("GetRequestsByStatus");

        requests.MapPost("", async ([FromBody] CreateRequestDto dto, IRequestService? requestService, ICurrentUserService currentUserService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                if (dto == null)
                    return Results.BadRequest();
                if (string.IsNullOrEmpty(dto.RequesterId))
                {
                    var accountPersonId = currentUserService.GetCurrentAccountPersonId();
                    if (string.IsNullOrEmpty(accountPersonId))
                        return Results.BadRequest(new { message = "RequesterId is required or account person not found" });
                    dto.RequesterId = accountPersonId;
                }
                var request = await requestService.CreateAsync(dto, ct);
                return Results.CreatedAtRoute("GetRequestById", new { id = request.Id }, request);
            })
            .WithName("CreateRequest");

        requests.MapPut("{id}", async (string id, [FromBody] UpdateRequestDto dto, IRequestService? requestService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                if (id != dto.Id)
                    return Results.BadRequest(new { message = "ID mismatch" });
                if (dto == null)
                    return Results.BadRequest();
                var request = await requestService.UpdateAsync(dto, ct);
                if (request == null)
                    return Results.NotFound(new { message = "Request not found or you don't have access" });
                return Results.Ok(request);
            })
            .WithName("UpdateRequest");

        requests.MapPost("{id}/approve", async (string id, [FromBody] ApproveRequestDto? dto, IRequestService? requestService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                var request = await requestService.ApproveAsync(id, dto?.AgreedCost, ct);
                if (request == null)
                    return Results.NotFound(new { message = "Request not found or you don't have access" });
                return Results.Ok(request);
            })
            .WithName("ApproveRequest");

        requests.MapPost("{id}/reject", async (string id, [FromBody] RejectRequestDto dto, IRequestService? requestService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                if (dto == null || string.IsNullOrWhiteSpace(dto.Reason))
                    return Results.BadRequest(new { message = "Reason is required" });
                var request = await requestService.RejectAsync(id, dto.Reason, ct);
                if (request == null)
                    return Results.NotFound(new { message = "Request not found or you don't have access" });
                return Results.Ok(request);
            })
            .WithName("RejectRequest");

        requests.MapDelete("{id}", async (string id, IRequestService? requestService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                await requestService.CancelAsync(id, ct);
                return Results.NoContent();
            })
            .WithName("CancelRequest");

        return group;
    }
}
