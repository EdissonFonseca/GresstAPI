using Gresst.Application.Queries;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class CustomerEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var parties = group.MapGroup("/customers")
            .WithTags("Customer");

        parties.MapGet("", async (
            IPartyService partyService,
            CancellationToken ct) =>
        {
            var result = await partyService.FindAsync(party => party.Roles.Contains(PartyRelationType.Customer), ct);
            return Results.Ok(result);
        }).WithName("GetCustomers");

        parties.MapGet("/{customerId}", async (
            string customerId,
            IPartyService partyService,
            CancellationToken ct) =>
        {
            var result = await partyService.GetByIdAsync(customerId, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).WithName("GetCustomerById");

        return group;
    }
}