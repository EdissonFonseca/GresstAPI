using Microsoft.AspNetCore.Routing;

namespace Gresst.API.Endpoints;

/// <summary>
/// Registers all Minimal API endpoints under /api/v1.
/// Legacy path /api/authentication/... is handled via URL rewrite to /api/v1/authentication/... (see Program.cs).
/// </summary>
public static class MapEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/v1")
            .WithTags("v1");

        AuthenticationEndpoints.Map(api);
        AuthorizationEndpoints.Map(api);
        AccountEndpoints.Map(api);
        CustomerEndpoints.Map(api);
        EmployeesEndpoints.Map(api);
        MeEndpoints.Map(api);
        PartyEndpoints.Map(api);
        SupplierEndpoints.Map(api);
        UserEndpoints.Map(api);

        return app;
    }
}
