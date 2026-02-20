using Gresst.Application.Constants;
using Gresst.Application.DTOs;
using Gresst.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class AuthorizationEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var authz = group.MapGroup("/authorization")
            .WithTags("Authorization")
            .RequireAuthorization();

        authz.MapGet("options", async (IAuthorizationService authzService, CancellationToken ct) =>
            {
                var options = await authzService.GetAllOptionsAsync(ct);
                return Results.Ok(options);
            })
            .WithName("GetAllOptions");

        authz.MapGet("options/{optionId}", async (string optionId, IAuthorizationService authzService, CancellationToken ct) =>
            {
                var option = await authzService.GetOptionByIdAsync(optionId, ct);
                if (option == null)
                    return Results.NotFound(new { error = "Option not found" });
                return Results.Ok(option);
            })
            .WithName("GetOption");

        authz.MapGet("options/{parentId}/children", async (string parentId, IAuthorizationService authzService, CancellationToken ct) =>
            {
                var options = await authzService.GetOptionsByParentAsync(parentId, ct);
                return Results.Ok(options);
            })
            .WithName("GetChildOptions");

        authz.MapPost("assign", async ([FromBody] AssignPermissionDto dto, IAuthorizationService authzService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var success = await authzService.AssignPermissionAsync(dto, ct);
                if (!success)
                    return Results.BadRequest(new { error = "Failed to assign permission" });
                return Results.Ok(new { message = "Permission assigned successfully" });
            })
            .RequireAuthorization(ApiRoles.PolicyAdminOnly)
            .WithName("AssignPermission");

        authz.MapGet("check", async ([FromQuery] string optionId, [FromQuery] PermissionFlags permission, IAuthorizationService authzService, CancellationToken ct) =>
            {
                var hasPermission = await authzService.CurrentUserHasPermissionAsync(optionId, permission, ct);
                return Results.Ok(new { hasPermission });
            })
            .WithName("CheckPermission");

        return group;
    }
}
