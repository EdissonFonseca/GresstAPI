using Gresst.Application.Services;

namespace Gresst.API.Endpoints;

public static class AccountEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var accounts = group.MapGroup("/accounts")
            .WithTags("Account")
            .RequireAuthorization();

        accounts.MapGet("{accountId}/users", async (string accountId, IUserService userService, CancellationToken ct) =>
            {
                var list = await userService.GetUsersByAccountAsync(accountId, ct);
                return Results.Ok(list);
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("GetUsersByAccount");

        return group;
    }
}
