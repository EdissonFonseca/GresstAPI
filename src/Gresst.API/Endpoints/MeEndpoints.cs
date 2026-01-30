using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class MeEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var me = group.MapGroup("/me")
            .WithTags("Me")
            .RequireAuthorization();

        me.MapGet("", async (IMeService meService, CancellationToken ct) =>
            {
                var context = await meService.GetCurrentContextAsync(ct);
                if (context == null)
                    return Results.NotFound(new { error = "User not found" });
                return Results.Ok(context);
            })
            .WithName("GetMe");

        me.MapPut("", async ([FromBody] UpdateUserProfileDto dto, System.Security.Claims.ClaimsPrincipal user, IUserService userService, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Results.Unauthorized();
                var u = await userService.UpdateUserProfileAsync(userIdClaim, dto, ct);
                if (u == null)
                    return Results.NotFound(new { error = "User not found" });
                return Results.Ok(u);
            })
            .WithName("UpdateMyProfile");

        me.MapPost("password", async ([FromBody] ChangePasswordDto dto, System.Security.Claims.ClaimsPrincipal user, IUserService userService, CancellationToken ct) =>
            {
                if (dto.NewPassword != dto.ConfirmPassword)
                    return Results.BadRequest(new { error = "Passwords do not match" });
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Results.Unauthorized();
                var success = await userService.ChangePasswordAsync(userIdClaim, dto, ct);
                if (!success)
                    return Results.BadRequest(new { error = "Current password is incorrect" });
                return Results.Ok(new { message = "Password updated successfully" });
            })
            .WithName("ChangeMyPassword");

        return group;
    }
}
