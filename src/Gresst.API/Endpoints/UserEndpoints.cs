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

        users.MapGet("me", async (IUserService userService, CancellationToken ct) =>
            {
                var user = await userService.GetCurrentUserAsync(ct);
                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(user);
            })
            .WithName("GetCurrentUser");

        users.MapGet("{id}", async (string id, IUserService userService, CancellationToken ct) =>
            {
                var user = await userService.GetUserByIdAsync(id, ct);
                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(user);
            })
            .WithName("GetUserById");

        users.MapGet("account/{accountId}", async (string accountId, IUserService userService, CancellationToken ct) =>
            {
                var list = await userService.GetUsersByAccountAsync(accountId, ct);
                return Results.Ok(list);
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("GetUsersByAccount");

        users.MapPost("", async ([FromBody] CreateUserDto dto, IUserService userService, CancellationToken ct) =>
            {
                if (dto == null)
                    return Results.BadRequest();
                var user = await userService.CreateUserAsync(dto, ct);
                return Results.CreatedAtRoute("GetUserById", new { id = user.Id }, user);
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("CreateUser");

        users.MapPut("me", async ([FromBody] UpdateUserProfileDto dto, System.Security.Claims.ClaimsPrincipal user, IUserService userService, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Results.Unauthorized();
                var u = await userService.UpdateUserProfileAsync(userIdClaim, dto, ct);
                if (u == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(u);
            })
            .WithName("UpdateMyProfile");

        users.MapPut("{id}", async (string id, [FromBody] UpdateUserProfileDto dto, IUserService userService, CancellationToken ct) =>
            {
                var user = await userService.UpdateUserProfileAsync(id, dto, ct);
                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(user);
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("UpdateUser");

        users.MapPost("me/change-password", async ([FromBody] ChangePasswordDto dto, System.Security.Claims.ClaimsPrincipal user, IUserService userService, CancellationToken ct) =>
            {
                if (dto.NewPassword != dto.ConfirmPassword)
                    return Results.BadRequest(new { error = "Las contraseñas no coinciden" });
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Results.Unauthorized();
                var success = await userService.ChangePasswordAsync(userIdClaim, dto, ct);
                if (!success)
                    return Results.BadRequest(new { error = "Contraseña actual incorrecta" });
                return Results.Ok(new { message = "Contraseña actualizada exitosamente" });
            })
            .WithName("ChangeMyPassword");

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
