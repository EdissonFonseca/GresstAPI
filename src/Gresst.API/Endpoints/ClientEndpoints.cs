using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class ClientEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var clients = group.MapGroup("/clients")
            .WithTags("Client");

        clients.MapGet("", async (IClientService clientService, ILogger<Program> logger, CancellationToken ct) =>
            {
                try
                {
                    var list = await clientService.GetAllAsync(ct);
                    foreach (var c in list)
                    {
                        c.IdPersona = c.Id ?? string.Empty;
                        c.Nombre = c.Name ?? string.Empty;
                    }
                    return Results.Ok(list);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error getting clients");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetClients");

        clients.MapGet("{id}", async (string id, IClientService clientService, ILogger<Program> logger, CancellationToken ct) =>
            {
                try
                {
                    var client = await clientService.GetByIdAsync(id, ct);
                    if (client == null)
                        return Results.NotFound();
                    client.IdPersona = client.Id ?? string.Empty;
                    client.Nombre = client.Name ?? string.Empty;
                    return Results.Ok(client);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error getting client");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetClientById");

        clients.MapPost("", ([FromBody] ClientDto cliente) => Results.Created())
            .WithName("PostClient");

        return group;
    }
}
