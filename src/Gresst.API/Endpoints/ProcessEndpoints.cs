using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class ProcessEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var processes = group.MapGroup("/processes")
            .WithTags("Process")
            .RequireAuthorization();

        processes.MapGet("transport-requests", async (IRequestService? requestService, IProcessService? processService, ICurrentUserService currentUserService, CancellationToken ct) =>
            {
                if (requestService == null)
                    return Results.StatusCode(503);
                if (processService == null)
                    return Results.StatusCode(503);
                var personId = currentUserService.GetCurrentAccountPersonId();
                if (string.IsNullOrEmpty(personId))
                    return Results.BadRequest(new { message = "Account person ID not found for current user" });
                var transportData = await requestService.GetMobileTransportWasteAsync(personId, ct);
                var list = await processService.MapTransportDataToProcessesAsync(transportData, ct);
                return Results.Ok(list);
            })
            .WithName("GetTransportRequests");

        return group;
    }
}
