using Gresst.Application.Constants;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Application.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class AccountEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var accounts = group.MapGroup("/accounts")
            .WithTags("Account")
            .RequireAuthorization();

        // Register new account with first administrator (anonymous)
        accounts.MapPost("register", async (
                [FromBody] RegisterAccountRequest request,
                IAccountRegistrationService registrationService,
                CancellationToken ct) =>
            {
                if (request == null)
                    return Results.BadRequest(new { error = "Request is required" });
                if (string.IsNullOrWhiteSpace(request.AdminEmail))
                    return Results.BadRequest(new { error = "AdminEmail is required" });
                if (string.IsNullOrWhiteSpace(request.AdminPassword))
                    return Results.BadRequest(new { error = "AdminPassword is required" });
                if (request.AdminPassword != request.ConfirmPassword)
                    return Results.BadRequest(new { error = "Password and confirmation do not match" });
                var adminPasswordValidation = PasswordValidator.Validate(request.AdminPassword);
                if (!adminPasswordValidation.IsValid)
                    return Results.BadRequest(new { error = "Password does not meet security requirements", validation = adminPasswordValidation });

                var result = await registrationService.RegisterAccountAsync(request, ct);
                if (result == null)
                    return Results.BadRequest(new { error = "Registration failed (e.g. email already in use or invalid PersonId)" });

                return Results.Created($"/api/v1/accounts/{result.AccountId}", result);
            })
            .AllowAnonymous()
            .WithName("RegisterAccount")
            .WithSummary("Register a new account with its first administrator user");

        accounts.MapGet("{accountId}/users", async (string accountId, IUserService userService, CancellationToken ct) =>
            {
                var list = await userService.GetUsersByAccountAsync(accountId, ct);
                return Results.Ok(list);
            })
            .RequireAuthorization(ApiRoles.PolicyAdminOnly)
            .WithName("GetUsersByAccount");

        return group;
    }
}
