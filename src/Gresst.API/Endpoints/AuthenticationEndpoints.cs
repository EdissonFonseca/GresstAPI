using Gresst.API;
using Gresst.Infrastructure.Authentication;
using Gresst.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class AuthenticationEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var auth = group.MapGroup("/authentication")
            .WithTags("Authentication");

        auth.MapGet("ping", () => Results.Ok(true))
            .AllowAnonymous()
            .WithName("Ping")
            .WithSummary("Health check");

        auth.MapGet("validatetoken", () => Results.Ok(true))
            .RequireAuthorization()
            .WithName("ValidateToken");

        auth.MapGet("isauthenticated", (System.Security.Claims.ClaimsPrincipal user) =>
            {
                if (!(user.Identity?.IsAuthenticated ?? false))
                    return Results.Unauthorized();
                var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                    return Results.BadRequest();
                var username = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                var accountId = user.FindFirst("AccountId")?.Value;
                var email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var roles = user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToArray();
                return Results.Ok(new { userId, username, accountId, email, roles, isAuthenticated = true });
            })
            .RequireAuthorization()
            .WithName("IsAuthenticated");

        auth.MapPost("login", async (
                [FromBody] LoginRequest request,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                var authService = factory.GetAuthenticationService();
                var result = await authService.LoginAsync(request, ct);
                if (!result.Success)
                    return Results.Unauthorized();
                return Results.Ok(result);
            })
            .AllowAnonymous()
            .WithName("Login");

        auth.MapPost("login/database", async (
                [FromBody] LoginRequest request,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                var authService = factory.GetDatabaseAuthenticationService();
                var result = await authService.LoginAsync(request, ct);
                if (!result.Success)
                    return Results.Unauthorized();
                return Results.Ok(result);
            })
            .AllowAnonymous()
            .WithName("LoginDatabase");

        auth.MapPost("login/external", async (
                [FromBody] LoginRequest request,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                var authService = factory.GetExternalAuthenticationService();
                var result = await authService.LoginAsync(request, ct);
                if (!result.Success)
                    return Results.Unauthorized();
                return Results.Ok(result);
            })
            .AllowAnonymous()
            .WithName("LoginExternal");

        auth.MapPost("authenticateforinterface", async (
                [FromBody] LoginRequest login,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                if (login == null)
                    return Results.BadRequest();
                var authService = factory.GetAuthenticationService();
                var (result, token) = await authService.IsUserAuthorizedForInterfaceAsync(login.Interface, login.Username, login.Token, ct);
                if (result && token != null)
                    return Results.Ok(token);
                var (result2, token2) = await authService.IsGuestAuthorizedForInterfaceAsync(login.Interface, login.Token, ct);
                if (result2 && token2 != null)
                    return Results.Ok(token2);
                return Results.Unauthorized();
            })
            .AllowAnonymous()
            .WithName("AuthenticateForInterface");

        auth.MapPost("validate", async (
                [FromBody] ValidateTokenRequest request,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                var authService = factory.GetAuthenticationService();
                var result = await authService.ValidateTokenAsync(request.Token, ct);
                if (!result.Success)
                    return Results.Unauthorized();
                return Results.Ok(result);
            })
            .WithName("ValidateTokenPost");

        auth.MapPost("refresh", async (
                [FromBody] RefreshTokenRequest request,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                if (request == null)
                    return Results.BadRequest();
                var authService = factory.GetAuthenticationService();
                var result = await authService.RefreshTokenAsync(request, ct);
                if (!result.Success)
                    return Results.Unauthorized();
                return Results.Ok(result);
            })
            .AllowAnonymous()
            .WithName("RefreshToken");

        auth.MapPost("isvalidrefreshtoken", async (
                [FromBody] LoginRequest login,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                if (login == null)
                    return Results.BadRequest();
                var authService = factory.GetAuthenticationService();
                var result = await authService.IsValidRefreshTokenAsync(login.Username, login.Token, ct);
                return result ? Results.Ok() : Results.Unauthorized();
            })
            .WithName("IsValidRefreshToken");

        auth.MapPost("existuser", async (
                [FromBody] LoginRequest login,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                if (login == null)
                    return Results.BadRequest();
                var authService = factory.GetAuthenticationService();
                var user = await authService.GetUserByEmailAsync(login.Username, ct);
                return user != null ? Results.Ok() : Results.Unauthorized();
            })
            .RequireAuthorization()
            .WithName("ExistUser");

        auth.MapGet("user", async (
                System.Security.Claims.ClaimsPrincipal user,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Results.BadRequest();
                var authService = factory.GetAuthenticationService();
                var u = await authService.GetUserByIdAsync(userId, ct);
                return u != null ? Results.Ok(u) : Results.NotFound();
            })
            .RequireAuthorization()
            .WithName("GetUser");

        auth.MapGet("me", (System.Security.Claims.ClaimsPrincipal user) =>
            {
                var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var username = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                var accountId = user.FindFirst("AccountId")?.Value;
                var email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var roles = user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToArray();
                return Results.Ok(new { userId, username, accountId, email, roles, isAuthenticated = true });
            })
            .RequireAuthorization()
            .WithName("Me");

        auth.MapGet("account", async (
                System.Security.Claims.ClaimsPrincipal user,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Results.Unauthorized();
                var authService = factory.GetAuthenticationService();
                var account = await authService.GetAccountIdForUserAsync(userIdClaim, ct);
                return Results.Ok(account);
            })
            .RequireAuthorization()
            .WithName("GetAccount");

        auth.MapPost("changepassword", async (
                [FromBody] LoginRequest userReq,
                System.Security.Claims.ClaimsPrincipal user,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                if (userReq == null)
                    return Results.BadRequest();
                var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Results.BadRequest();
                var authService = factory.GetAuthenticationService();
                var changed = await authService.ChangePasswordAsync(userId, userReq.Password, ct);
                return changed ? Results.Ok() : Results.NotFound();
            })
            .RequireAuthorization()
            .WithName("ChangePassword");

        auth.MapPost("changename", async (
                [FromBody] LoginRequest userReq,
                System.Security.Claims.ClaimsPrincipal user,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                if (userReq == null)
                    return Results.BadRequest();
                var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Results.BadRequest();
                var authService = factory.GetAuthenticationService();
                var changed = await authService.ChangePasswordAsync(userId, userReq.Name, ct);
                return changed ? Results.Ok() : Results.NotFound();
            })
            .RequireAuthorization()
            .WithName("ChangeName");

        auth.MapPost("logout", async (
                [FromBody] LogoutRequest? request,
                System.Security.Claims.ClaimsPrincipal user,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var authService = factory.GetAuthenticationService();
                    await authService.LogoutAsync(userIdClaim, request?.RefreshToken, ct);
                }
                return Results.Ok(new { message = "Logged out successfully" });
            })
            .RequireAuthorization()
            .WithName("Logout");

        return group;
    }
}
