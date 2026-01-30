using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class ResourceAssignmentEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var resourceAssignment = group.MapGroup("/resourceassignment")
            .WithTags("ResourceAssignment")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        resourceAssignment.MapGet("users/{userId}/facilities", async (string userId, IDataSegmentationService segmentationService, CancellationToken ct) =>
            {
                var facilityIds = await segmentationService.GetUserFacilityIdsAsync(ct);
                return Results.Ok(facilityIds);
            })
            .WithName("GetUserFacilities");

        resourceAssignment.MapPost("users/{userId}/facilities/{facilityId}", async (string userId, string facilityId, IDataSegmentationService segmentationService, CancellationToken ct) =>
            {
                var success = await segmentationService.AssignFacilityToUserAsync(userId, facilityId, ct);
                if (!success) return Results.BadRequest(new { error = "Assignment already exists or failed" });
                return Results.Ok(new { message = "Facility assigned to user successfully" });
            })
            .WithName("AssignFacilityToUser");

        resourceAssignment.MapDelete("users/{userId}/facilities/{facilityId}", async (string userId, string facilityId, IDataSegmentationService segmentationService, CancellationToken ct) =>
            {
                var success = await segmentationService.RevokeFacilityFromUserAsync(userId, facilityId, ct);
                if (!success) return Results.NotFound(new { error = "Assignment not found" });
                return Results.Ok(new { message = "Facility revoked from user successfully" });
            })
            .WithName("RevokeFacilityFromUser");

        resourceAssignment.MapGet("users/{userId}/facilities/{facilityId}/check", async (string userId, string facilityId, IDataSegmentationService segmentationService, CancellationToken ct) =>
            {
                var hasAccess = await segmentationService.UserHasAccessToFacilityAsync(facilityId, ct);
                return Results.Ok(new { hasAccess });
            })
            .WithName("CheckFacilityAccess");

        resourceAssignment.MapGet("users/{userId}/vehicles", async (string userId, IDataSegmentationService segmentationService, CancellationToken ct) =>
            {
                var vehicleIds = await segmentationService.GetUserVehicleIdsAsync(ct);
                return Results.Ok(vehicleIds);
            })
            .WithName("GetUserVehicles");

        resourceAssignment.MapPost("users/{userId}/vehicles/{vehicleId}", async (string userId, string vehicleId, IDataSegmentationService segmentationService, CancellationToken ct) =>
            {
                var success = await segmentationService.AssignVehicleToUserAsync(userId, vehicleId, ct);
                if (!success) return Results.BadRequest(new { error = "Assignment already exists or failed" });
                return Results.Ok(new { message = "Vehicle assigned to user successfully" });
            })
            .WithName("AssignVehicleToUser");

        resourceAssignment.MapDelete("users/{userId}/vehicles/{vehicleId}", async (string userId, string vehicleId, IDataSegmentationService segmentationService, CancellationToken ct) =>
            {
                var success = await segmentationService.RevokeVehicleFromUserAsync(userId, vehicleId, ct);
                if (!success) return Results.NotFound(new { error = "Assignment not found" });
                return Results.Ok(new { message = "Vehicle revoked from user successfully" });
            })
            .WithName("RevokeVehicleFromUser");

        resourceAssignment.MapGet("users/{userId}/vehicles/{vehicleId}/check", async (string userId, string vehicleId, IDataSegmentationService segmentationService, CancellationToken ct) =>
            {
                var hasAccess = await segmentationService.UserHasAccessToVehicleAsync(vehicleId, ct);
                return Results.Ok(new { hasAccess });
            })
            .WithName("CheckVehicleAccess");

        return group;
    }
}
