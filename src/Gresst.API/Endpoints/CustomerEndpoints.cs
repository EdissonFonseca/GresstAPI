using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class CustomerEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var customers = group.MapGroup("/customers")
            .WithTags("Customer");

        customers.MapGet("", async (ICustomerService customerService, ILogger<Program> logger, CancellationToken ct) =>
            {
                try
                {
                    var list = await customerService.GetAllAsync(ct);
                    return Results.Ok(list);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error getting customers");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetCustomers");

        customers.MapGet("{id}", async (string id, ICustomerService customerService, ILogger<Program> logger, CancellationToken ct) =>
            {
                try
                {
                    var customer = await customerService.GetByIdAsync(id, ct);
                    if (customer == null)
                        return Results.NotFound();
                    return Results.Ok(customer);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error getting customer");
                    return Results.StatusCode(500);
                }
            })
            .WithName("GetCustomerById");

        customers.MapPost("", ([FromBody] CustomerDto customer) => Results.Created())
            .WithName("PostCustomer");

        return group;
    }
}
