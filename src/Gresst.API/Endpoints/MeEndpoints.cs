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

        // Full context: profile + account + person (one round-trip; preferred for app shell)
        me.MapGet("", async (IMeService meService, CancellationToken ct) =>
            {
                var context = await meService.GetCurrentContextAsync(ct);
                if (context == null)
                    return Results.NotFound(new { error = "User not found" });
                return Results.Ok(context);
            })
            .WithName("GetMe")
            .WithSummary("Current user full context (profile, account, and person)");

        // Profile only: user data without account or person
        me.MapGet("profile", async (IMeService meService, CancellationToken ct) =>
            {
                var context = await meService.GetCurrentContextAsync(ct);
                if (context == null)
                    return Results.NotFound(new { error = "User not found" });
                return Results.Ok(context.Profile);
            })
            .WithName("GetMyProfile")
            .WithSummary("Current user profile only (no account or person)");

        // Account for the current user (when only account is needed)
        me.MapGet("account", async (IMeService meService, CancellationToken ct) =>
            {
                var context = await meService.GetCurrentContextAsync(ct);
                if (context == null || context.Account == null)
                    return Results.NotFound(new { error = "Account not found" });
                return Results.Ok(context.Account);
            })
            .WithName("GetMyAccount")
            .WithSummary("Account corresponding to the current user");

        // Person for the current user (user's linked person or account's legal rep)
        me.MapGet("person", async (IMeService meService, CancellationToken ct) =>
            {
                var context = await meService.GetCurrentContextAsync(ct);
                if (context == null || context.Person == null)
                    return Results.NotFound(new { error = "Person not found" });
                return Results.Ok(context.Person);
            })
            .WithName("GetMyPerson")
            .WithSummary("Person corresponding to the current user");

        // Update full profile (PUT /me)
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
            .WithName("UpdateMyProfile")
            .WithSummary("Update current user profile");

        // Update profile (same as PUT /me; preferred path for profile updates)
        me.MapPut("profile", async ([FromBody] UpdateUserProfileDto dto, System.Security.Claims.ClaimsPrincipal user, IUserService userService, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Results.Unauthorized();
                var u = await userService.UpdateUserProfileAsync(userIdClaim, dto, ct);
                if (u == null)
                    return Results.NotFound(new { error = "User not found" });
                return Results.Ok(u);
            })
            .WithName("UpdateMyProfileByPath")
            .WithSummary("Update current user profile (name, email, etc.)");

        return group;
    }
}
