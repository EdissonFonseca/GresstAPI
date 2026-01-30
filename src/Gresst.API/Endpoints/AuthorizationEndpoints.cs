using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class AuthorizationEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var authz = group.MapGroup("/authorization")
            .WithTags("Authorization")
            .RequireAuthorization();

        authz.MapGet("options", async (Gresst.Application.Services.IAuthorizationService authzService, CancellationToken ct) =>
            {
                var options = await authzService.GetAllOptionsAsync(ct);
                return Results.Ok(options);
            })
            .WithName("GetAllOptions");

        authz.MapGet("options/{optionId}", async (string optionId, Gresst.Application.Services.IAuthorizationService authzService, CancellationToken ct) =>
            {
                var option = await authzService.GetOptionByIdAsync(optionId, ct);
                if (option == null)
                    return Results.NotFound(new { error = "Option not found" });
                return Results.Ok(option);
            })
            .WithName("GetOption");

        authz.MapGet("options/{parentId}/children", async (string parentId, Gresst.Application.Services.IAuthorizationService authzService, CancellationToken ct) =>
            {
                var options = await authzService.GetOptionsByParentAsync(parentId, ct);
                return Results.Ok(options);
            })
            .WithName("GetChildOptions");

        authz.MapGet("users/{userId}/permissions", async (string userId, Gresst.Application.Services.IAuthorizationService authzService, CancellationToken ct) =>
            {
                var permissions = await authzService.GetUserPermissionsAsync(userId, ct);
                return Results.Ok(permissions);
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("GetUserPermissions");

        authz.MapGet("me/permissions", async (System.Security.Claims.ClaimsPrincipal user, Gresst.Application.Services.IAuthorizationService authzService, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Results.Unauthorized();
                var permissions = await authzService.GetUserPermissionsAsync(userIdClaim, ct);
                return Results.Ok(permissions);
            })
            .WithName("GetMyPermissions");

        authz.MapGet("users/{userId}/permissions/{optionId}", async (string userId, string optionId, Gresst.Application.Services.IAuthorizationService authzService, CancellationToken ct) =>
            {
                var permission = await authzService.GetUserPermissionAsync(userId, optionId, ct);
                if (permission == null)
                    return Results.NotFound(new { error = "Permission not found" });
                return Results.Ok(permission);
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("GetUserPermission");

        authz.MapPost("assign", async ([FromBody] AssignPermissionDto dto, Gresst.Application.Services.IAuthorizationService authzService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var success = await authzService.AssignPermissionAsync(dto, ct);
                if (!success)
                    return Results.BadRequest(new { error = "Failed to assign permission" });
                return Results.Ok(new { message = "Permission assigned successfully" });
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("AssignPermission");

        authz.MapPut("users/{userId}/permissions/{optionId}", async (string userId, string optionId, [FromBody] AssignPermissionDto dto, Gresst.Application.Services.IAuthorizationService authzService, CancellationToken ct) =>
            {
                var success = await authzService.UpdatePermissionAsync(userId, optionId, dto, ct);
                if (!success)
                    return Results.NotFound(new { error = "Permission not found" });
                return Results.Ok(new { message = "Permission updated successfully" });
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("UpdatePermission");

        authz.MapDelete("users/{userId}/permissions/{optionId}", async (string userId, string optionId, Gresst.Application.Services.IAuthorizationService authzService, CancellationToken ct) =>
            {
                var success = await authzService.RevokePermissionAsync(userId, optionId, ct);
                if (!success)
                    return Results.NotFound(new { error = "Permission not found" });
                return Results.Ok(new { message = "Permission revoked successfully" });
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("RevokePermission");

        authz.MapGet("check", async ([FromQuery] string optionId, [FromQuery] PermissionFlags permission, Gresst.Application.Services.IAuthorizationService authzService, CancellationToken ct) =>
            {
                var hasPermission = await authzService.CurrentUserHasPermissionAsync(optionId, permission, ct);
                return Results.Ok(new { hasPermission });
            })
            .WithName("CheckPermission");

        return group;
    }
}
