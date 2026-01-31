using Gresst.Application.DTOs;
using Gresst.Application.Services;
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
                    return Results.BadRequest(new { error = "Query parameter 'email' is required" });
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
                var isAdmin = currentUser.Roles?.Contains("Admin", StringComparer.OrdinalIgnoreCase) == true;
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
                var user = await userService.CreateUserAsync(dto, ct);
                return Results.CreatedAtRoute("GetUserById", new { id = user.Id }, user);
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("CreateUser");

        users.MapPut("{id}", async (string id, [FromBody] UpdateUserProfileDto dto, IUserService userService, CancellationToken ct) =>
            {
                var user = await userService.UpdateUserProfileAsync(id, dto, ct);
                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(user);
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("UpdateUser");

        users.MapPost("{id}/deactivate", async (string id, IUserService userService, CancellationToken ct) =>
            {
                var success = await userService.DeactivateUserAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(new { message = "Usuario desactivado" });
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("DeactivateUser");

        users.MapPost("{id}/activate", async (string id, IUserService userService, CancellationToken ct) =>
            {
                var success = await userService.ActivateUserAsync(id, ct);
                if (!success)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(new { message = "Usuario activado" });
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("ActivateUser");

        return group;
    }
}
