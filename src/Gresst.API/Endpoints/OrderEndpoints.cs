using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class OrderEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var orders = group.MapGroup("/orders")
            .WithTags("Order")
            .RequireAuthorization();

        orders.MapGet("", async (IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                var list = await orderService.GetAllAsync(ct);
                return Results.Ok(list);
            })
            .WithName("GetAllOrders");

        orders.MapGet("{id}", async (string id, IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                var order = await orderService.GetByIdAsync(id, ct);
                if (order == null)
                    return Results.NotFound(new { message = "Order not found" });
                return Results.Ok(order);
            })
            .WithName("GetOrderById");

        orders.MapPost("", async ([FromBody] CreateOrderDto dto, IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                if (dto == null)
                    return Results.BadRequest();
                var order = await orderService.CreateAsync(dto, ct);
                return Results.CreatedAtRoute("GetOrderById", new { id = order.Id }, order);
            })
            .WithName("CreateOrder");

        orders.MapPut("{id}", async (string id, [FromBody] CreateOrderDto dto, IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                if (dto == null)
                    return Results.BadRequest();
                var updateDto = new { Id = id, dto.Type, dto.ProviderId, dto.CustomerId, dto.RequestId, dto.ScheduledDate, dto.Description, dto.EstimatedCost, dto.VehicleId, dto.FacilityId, dto.RouteId, dto.Items };
                var order = await orderService.UpdateAsync(updateDto, ct);
                if (order == null)
                    return Results.NotFound(new { message = "Order not found" });
                return Results.Ok(order);
            })
            .WithName("UpdateOrder");

        orders.MapGet("provider/{providerId}", async (string providerId, IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                if (string.IsNullOrEmpty(providerId))
                    return Results.BadRequest(new { message = "Invalid provider ID" });
                var list = await orderService.GetByProviderAsync(providerId, ct);
                return Results.Ok(list);
            })
            .WithName("GetOrdersByProvider");

        orders.MapGet("customer/{customerId}", async (string customerId, IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                if (string.IsNullOrEmpty(customerId))
                    return Results.BadRequest(new { message = "Invalid customer ID" });
                var list = await orderService.GetByCustomerAsync(customerId, ct);
                return Results.Ok(list);
            })
            .WithName("GetOrdersByCustomer");

        orders.MapGet("status/{status}", async (string status, IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                if (string.IsNullOrWhiteSpace(status))
                    return Results.BadRequest(new { message = "Status is required" });
                var list = await orderService.GetByStatusAsync(status, ct);
                return Results.Ok(list);
            })
            .WithName("GetOrdersByStatus");

        orders.MapGet("scheduled", async ([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                if (startDate > endDate)
                    return Results.BadRequest(new { message = "Start date must be before end date" });
                var list = await orderService.GetScheduledAsync(startDate, endDate, ct);
                return Results.Ok(list);
            })
            .WithName("GetScheduledOrders");

        orders.MapPost("{id}/schedule", async (string id, [FromBody] ScheduleOrderDto dto, IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                if (string.IsNullOrEmpty(id))
                    return Results.BadRequest(new { message = "Invalid order ID" });
                var order = await orderService.ScheduleAsync(id, dto.ScheduledDate, dto.VehicleId, dto.RouteId, ct);
                if (order == null)
                    return Results.NotFound(new { message = "Order not found" });
                return Results.Ok(order);
            })
            .WithName("ScheduleOrder");

        orders.MapPost("{id}/start", async (string id, IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                if (string.IsNullOrEmpty(id))
                    return Results.BadRequest(new { message = "Invalid order ID" });
                var order = await orderService.StartAsync(id, ct);
                if (order == null)
                    return Results.NotFound(new { message = "Order not found" });
                return Results.Ok(order);
            })
            .WithName("StartOrder");

        orders.MapPost("{id}/complete", async (string id, [FromBody] CompleteOrderDto? dto, IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                if (string.IsNullOrEmpty(id))
                    return Results.BadRequest(new { message = "Invalid order ID" });
                var order = await orderService.CompleteAsync(id, dto?.ActualCost, ct);
                if (order == null)
                    return Results.NotFound(new { message = "Order not found" });
                return Results.Ok(order);
            })
            .WithName("CompleteOrder");

        orders.MapPost("{id}/cancel", async (string id, [FromBody] CancelOrderDto? dto, IOrderService? orderService, CancellationToken ct) =>
            {
                if (orderService == null)
                    return Results.StatusCode(503);
                if (string.IsNullOrEmpty(id))
                    return Results.BadRequest(new { message = "Invalid order ID" });
                await orderService.CancelAsync(id, dto?.Reason, ct);
                return Results.Ok(new { message = "Order cancelled successfully" });
            })
            .WithName("CancelOrder");

        return group;
    }
}
