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

        // Full context: profile, account, party, roles, and permissions (one round-trip; preferred for app shell)
        me.MapGet("", async (IMeService meService, CancellationToken ct) =>
            {
                var context = await meService.GetCurrentContextAsync(ct);
                if (context == null)
                    return Results.NotFound(new { error = "User not found" });
                return Results.Ok(context);
            })
            .WithName("GetMe")
            .WithSummary("Current user full context (profile, account, person, roles, and permissions)");

        // Profile only: user data without account or person
        me.MapGet("profile", async (IMeService meService, HttpContext httpContext, IConfiguration configuration, CancellationToken ct) =>
            {
                var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
                var hasBearer = !string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);
                var cookieName = configuration["Authentication:AccessTokenCookieName"] ?? "access_token";
                var hasCookie = httpContext.Request.Cookies.TryGetValue(cookieName, out var cookieValue) && !string.IsNullOrEmpty(cookieValue);
                var authSource = httpContext.Items["AuthSource"]?.ToString() ?? "(not set)";
                var loggerFactory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("Gresst.API.Endpoints.MeEndpoints");
                logger.LogInformation(
                    "[me/profile] Auth: Bearer={HasBearer}, Cookie({CookieName})={HasCookie}, AuthSource={AuthSource}",
                    hasBearer, cookieName, hasCookie, authSource);

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
                if (context == null || context.Party == null)
                    return Results.NotFound(new { error = "Person not found" });
                return Results.Ok(context.Party);
            })
            .WithName("GetMyPerson")
            .WithSummary("Person corresponding to the current user");

        // Roles for the current user (separate from profile to keep profile lean)
        me.MapGet("roles", async (IUserService userService, CancellationToken ct) =>
            {
                var currentUser = await userService.GetCurrentUserAsync(ct);
                if (currentUser == null)
                    return Results.Unauthorized();
                return Results.Ok(currentUser.Roles ?? Array.Empty<string>());
            })
            .WithName("GetMyRoles")
            .WithSummary("Current user roles")
            .WithDescription("Returns the list of roles for the currently authenticated user. Profile (GET /me, GET /me/profile) does not include roles.");

        // Permissions for the current user (best practice: under /me for current-user context)
        me.MapGet("permissions", async (System.Security.Claims.ClaimsPrincipal user, IAuthorizationService authzService, CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Results.Unauthorized();
                var permissions = await authzService.GetUserPermissionsAsync(userIdClaim, ct);
                return Results.Ok(permissions);
            })
            .WithName("GetMyPermissions")
            .WithSummary("Current user permissions")
            .WithDescription("Returns the list of permissions for the currently authenticated user. Prefer this over authorization/users/{userId}/permissions when the caller is asking for their own permissions.");

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
