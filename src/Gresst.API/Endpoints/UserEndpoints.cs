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

        users.MapGet("by-email/{email}", async (string email, IUserService userService, CancellationToken ct) =>
            {
                var user = await userService.GetUserByEmailAsync(email, ct);
                if (user == null)
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                return Results.Ok(user);
            })
            .WithName("GetUserByEmail");

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
