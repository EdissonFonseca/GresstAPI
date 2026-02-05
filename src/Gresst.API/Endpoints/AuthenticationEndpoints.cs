using Gresst.API;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Infrastructure.Authentication;
using Gresst.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gresst.API.Endpoints;

public static class AuthenticationEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var auth = group.MapGroup("/authentication")
            .WithTags("Authentication");


        // Human login (database or external based on config)
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
            .WithName("Login")
            .WithSummary("Login with username/password; returns access and refresh tokens");

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
            .WithName("LoginDatabase")
            .WithSummary("Login against database only; returns access and refresh tokens");

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
            .WithName("LoginExternal")
            .WithSummary("Login via external provider; returns access and refresh tokens");

        // Register a new user under an existing account (anonymous)
        auth.MapPost("register", async (
                [FromBody] RegisterRequest request,
                IUserService userService,
                CancellationToken ct) =>
            {
                if (request == null)
                    return Results.BadRequest(new { error = "Request is required" });
                if (string.IsNullOrWhiteSpace(request.AccountId))
                    return Results.BadRequest(new { error = "AccountId is required" });
                if (string.IsNullOrWhiteSpace(request.Email))
                    return Results.BadRequest(new { error = "Email is required" });
                if (string.IsNullOrWhiteSpace(request.Password))
                    return Results.BadRequest(new { error = "Password is required" });
                if (request.Password != request.ConfirmPassword)
                    return Results.BadRequest(new { error = "Password and confirmation do not match" });
                if (request.Password.Length < 6)
                    return Results.BadRequest(new { error = "Password must be at least 6 characters" });

                var accountExists = await userService.AccountExistsAsync(request.AccountId, ct);
                if (!accountExists)
                    return Results.BadRequest(new { error = "Account does not exist" });

                var existingUser = await userService.GetUserByEmailAsync(request.Email, ct);
                if (existingUser != null)
                    return Results.BadRequest(new { error = "Email is already in use" });

                var createDto = new CreateUserDto
                {
                    AccountId = request.AccountId,
                    Name = string.IsNullOrWhiteSpace(request.Name) ? request.Email : request.Name,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = request.Password,
                    PersonId = request.PersonId,
                    Roles = new[] { Gresst.Application.Constants.ApiRoles.DefaultRole }
                };

                var user = await userService.CreateUserAsync(createDto, ct);
                return Results.CreatedAtRoute("GetUserById", new { id = user.Id }, user);
            })
            .AllowAnonymous()
            .WithName("Register")
            .WithSummary("Register a new user under an existing account");

        // Service/client (machine-to-machine) token. Client Credentials only: client_id + client_secret.
        auth.MapPost("service/token", async (
                [FromBody] ServiceTokenRequest request,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                if (request == null || string.IsNullOrWhiteSpace(request.ClientId) || string.IsNullOrWhiteSpace(request.ClientSecret))
                    return Results.BadRequest(new { error = "client_id and client_secret are required" });
                var authService = factory.GetDatabaseAuthenticationService();
                var result = await authService.IssueServiceTokenAsync(request, ct);
                if (result == null)
                    return Results.Unauthorized();
                return Results.Ok(new ServiceTokenResponse
                {
                    AccessToken = result.AccessToken,
                    TokenType = result.TokenType,
                    ExpiresIn = result.ExpiresInSeconds,
                    SubjectType = ClaimConstants.SubjectTypeService
                });
            })
            .AllowAnonymous()
            .WithName("GetServiceToken")
            .WithSummary("Obtain an access token for a service (Client Credentials: client_id + client_secret)")
            .WithDescription("Send client_id (Usuario.Correo) and client_secret (Usuario.Clave; same encryption as user passwords). User must be active (IdEstado = A). Scopes in Usuario.DatosAdicionales as JSON: {\"scopes\": [\"read:data\", \"write:logs\"]}. Token always includes exp.");

        // Validate access token: GET with Bearer token; returns 200 if token is valid and active, 401 otherwise
        auth.MapGet("token/validate", (ClaimsPrincipal user) =>
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();
                return Results.Ok(new { valid = true, authenticated = true });
            })
            .RequireAuthorization()
            .WithName("ValidateAccessToken")
            .WithSummary("Validate that the access token (Bearer) is still active; returns 200 if authenticated, 401 if missing or expired");

        // Exchange refresh token for new access + refresh tokens
        auth.MapPost("token/refresh", async (
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
            .WithName("RefreshToken")
            .WithSummary("Exchange refresh token for new access and refresh tokens");

        // Check whether a refresh token is still valid (no new tokens issued)
        auth.MapPost("refresh-token/validate", async (
                [FromBody] ValidateRefreshTokenRequest request,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.RefreshToken))
                    return Results.BadRequest();
                var authService = factory.GetAuthenticationService();
                var result = await authService.IsValidRefreshTokenAsync(request.Username, request.RefreshToken, ct);
                return result ? Results.Ok(new { valid = true }) : Results.Unauthorized();
            })
            .AllowAnonymous()
            .WithName("ValidateRefreshToken")
            .WithSummary("Check if a refresh token is still valid");

        // Forgot password: request a reset token (always returns success to avoid email enumeration)
        auth.MapPost("forgot-password", async (
                [FromBody] ForgotPasswordRequest request,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Email))
                    return Results.BadRequest(new { error = "Email is required" });
                var authService = factory.GetAuthenticationService();
                await authService.RequestPasswordResetAsync(request.Email, ct);
                return Results.Ok(new { message = "If an account exists for this email, you will receive instructions to reset your password." });
            })
            .AllowAnonymous()
            .WithName("ForgotPassword")
            .WithSummary("Request a password reset; always returns success to avoid email enumeration");

        // Reset password: set new password using the token from forgot-password flow
        auth.MapPost("reset-password", async (
                [FromBody] ResetPasswordRequest request,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Token))
                    return Results.BadRequest(new { error = "Token is required" });
                if (string.IsNullOrWhiteSpace(request.NewPassword))
                    return Results.BadRequest(new { error = "New password is required" });
                if (request.NewPassword != request.ConfirmPassword)
                    return Results.BadRequest(new { error = "New password and confirmation do not match" });
                var authService = factory.GetAuthenticationService();
                var success = await authService.ResetPasswordAsync(request.Token, request.NewPassword, ct);
                if (!success)
                    return Results.BadRequest(new { error = "Invalid or expired reset token" });
                return Results.Ok(new { message = "Password has been reset successfully." });
            })
            .AllowAnonymous()
            .WithName("ResetPassword")
            .WithSummary("Set new password using the token received by email");

        // Change password (logged-in user; requires current password)
        auth.MapPost("change-password", async (
                [FromBody] ChangePasswordDto request,
                ClaimsPrincipal user,
                IUserService userService,
                CancellationToken ct) =>
            {
                if (request == null)
                    return Results.BadRequest(new { error = "Request is required" });
                if (request.NewPassword != request.ConfirmPassword)
                    return Results.BadRequest(new { error = "New password and confirmation do not match" });
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Results.Unauthorized();
                var success = await userService.ChangePasswordAsync(userIdClaim, request, ct);
                if (!success)
                    return Results.BadRequest(new { error = "Current password is incorrect" });
                return Results.Ok(new { message = "Password updated successfully" });
            })
            .RequireAuthorization()
            .WithName("ChangePassword")
            .WithSummary("Change password for the current user (requires current password)");

        auth.MapPost("logout", async (
                [FromBody] LogoutRequest? request,
                ClaimsPrincipal user,
                AuthenticationServiceFactory factory,
                CancellationToken ct) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var authService = factory.GetAuthenticationService();
                    await authService.LogoutAsync(userIdClaim, request?.RefreshToken, ct);
                }
                return Results.Ok(new { message = "Logged out successfully" });
            })
            .RequireAuthorization()
            .WithName("Logout")
            .WithSummary("Revoke refresh token(s) for the current user");

        return group;
    }
}
