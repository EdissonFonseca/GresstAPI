using Gresst.Application.Constants;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Application.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var users = group.MapGroup("/users")
            .WithTags("User")
            .RequireAuthorization();

        // Get user by email (query param to avoid path encoding issues with @, +, etc.)
        users.MapGet("", async ([FromQuery] string? email, IUserService userService, CancellationToken ct) =>
            {
                if (string.IsNullOrWhiteSpace(email))
                    return Results.BadRequest(new { error = "Query parameter 'emaidotl' is required" });
                var user = await userService.GetUserByEmailAsync(email, ct);
                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(user);
            })
            .WithName("GetUserByEmail")
            .WithSummary("Get user by email")
            .WithDescription("Returns a single user by email. Use query parameter: GET /users?email=user@example.com. Required to avoid encoding issues with @ or + in the path.");

        // All users of the authenticated user's account (only if the authenticated user is Admin)
        users.MapGet("account", async (IUserService userService, CancellationToken ct) =>
            {
                var currentUser = await userService.GetCurrentUserAsync(ct);
                if (currentUser == null)
                    return Results.Unauthorized();
                var isAdmin = ApiRoles.IsAdmin(currentUser.Roles);
                if (!isAdmin)
                    return Results.Forbid();
                var list = await userService.GetUsersByAccountAsync(currentUser.AccountId, ct);
                return Results.Ok(list);
            })
            .WithName("GetUsersOfMyAccount")
            .WithSummary("Get all users of the current user's account (Admin only)");

        users.MapGet("{id}", async (string id, IUserService userService, CancellationToken ct) =>
            {
                var user = await userService.GetUserByIdAsync(id, ct);
                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(user);
            })
            .WithName("GetUserById");

        users.MapPost("", async ([FromBody] CreateUserDto dto, IUserService userService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                if (!string.IsNullOrEmpty(dto.Password))
                {
                    var createUserPasswordValidation = PasswordValidator.Validate(dto.Password);
                    if (!createUserPasswordValidation.IsValid)
                        return Results.BadRequest(new { error = "Password does not meet security requirements", validation = createUserPasswordValidation });
                }
                var user = await userService.CreateUserAsync(dto, ct);
                return Results.CreatedAtRoute("GetUserById", new { id = user.Id }, user);
            })
            .RequireAuthorization(ApiRoles.PolicyAdminOnly)
            .WithName("CreateUser");

        users.MapPut("{id}", async (string id, [FromBody] UpdateUserProfileDto dto, IUserService userService, CancellationToken ct) =>
            {
                var user = await userService.UpdateUserProfileAsync(id, dto, ct);
                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(user);
            })
            .RequireAuthorization(ApiRoles.PolicyAdminOnly)
            .WithName("UpdateUser");

        users.MapPost("{id}/deactivate", async (string id, IUserService userService, CancellationToken ct) =>
            {
                var success = await userService.DeactivateUserAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(new { message = "Usuario desactivado" });
            })
            .RequireAuthorization(ApiRoles.PolicyAdminOnly)
            .WithName("DeactivateUser");

        users.MapPost("{id}/activate", async (string id, IUserService userService, CancellationToken ct) =>
            {
                var success = await userService.ActivateUserAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(new { message = "Usuario activado" });
            })
            .RequireAuthorization(ApiRoles.PolicyAdminOnly)
            .WithName("ActivateUser");

        // Permissions for a given user (best practice: under /users for user resource sub-resource)
        users.MapGet("{userId}/permissions", async (string userId, IAuthorizationService authzService, CancellationToken ct) =>
            {
                var permissions = await authzService.GetUserPermissionsAsync(userId, ct);
                return Results.Ok(permissions);
            })
            .RequireAuthorization(ApiRoles.PolicyAdminOnly)
            .WithName("GetUserPermissions")
            .WithSummary("Get permissions for a user (Admin only)")
            .WithDescription("Returns the list of permissions for the specified user. For the current user's permissions use GET /me/permissions.");

        users.MapGet("{userId}/permissions/{optionId}", async (string userId, string optionId, IAuthorizationService authzService, CancellationToken ct) =>
            {
                var permission = await authzService.GetUserPermissionAsync(userId, optionId, ct);
                if (permission == null)
                    return Results.NotFound(new { error = "Permission not found" });
                return Results.Ok(permission);
            })
            .RequireAuthorization(ApiRoles.PolicyAdminOnly)
            .WithName("GetUserPermission")
            .WithSummary("Get a specific permission for a user (Admin only)");

        users.MapPut("{userId}/permissions/{optionId}", async (string userId, string optionId, [FromBody] AssignPermissionDto dto, IAuthorizationService authzService, CancellationToken ct) =>
            {
                var success = await authzService.UpdatePermissionAsync(userId, optionId, dto, ct);
                if (!success)
                    return Results.NotFound(new { error = "Permission not found" });
                return Results.Ok(new { message = "Permission updated successfully" });
            })
            .RequireAuthorization(ApiRoles.PolicyAdminOnly)
            .WithName("UpdateUserPermission")
            .WithSummary("Update a permission for a user (Admin only)");

        users.MapDelete("{userId}/permissions/{optionId}", async (string userId, string optionId, IAuthorizationService authzService, CancellationToken ct) =>
            {
                var success = await authzService.RevokePermissionAsync(userId, optionId, ct);
                if (!success)
                    return Results.NotFound(new { error = "Permission not found" });
                return Results.Ok(new { message = "Permission revoked successfully" });
            })
            .RequireAuthorization(ApiRoles.PolicyAdminOnly)
            .WithName("RevokeUserPermission")
            .WithSummary("Revoke a permission for a user (Admin only)");

        return group;
    }
}
