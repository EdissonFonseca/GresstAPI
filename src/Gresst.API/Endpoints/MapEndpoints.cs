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
        //CustomerEndpoints.Map(api);
        //FacilityEndpoints.Map(api);
        //WasteEndpoints.Map(api);
        //ManagementEndpoints.Map(api);
        //MaterialEndpoints.Map(api);
        //VehicleEndpoints.Map(api);
        //PackagingEndpoints.Map(api);
        //SupplyEndpoints.Map(api);
        //ServiceEndpoints.Map(api);
        //WasteClassEndpoints.Map(api);
        //TreatmentEndpoints.Map(api);
        //RouteEndpoints.Map(api);
        //OrderEndpoints.Map(api);
        //RequestEndpoints.Map(api);
        //ProcessEndpoints.Map(api);
        //CollectionsEndpoints.Map(api);
        AccountEndpoints.Map(api);
        MeEndpoints.Map(api);
        UserEndpoints.Map(api);
        //PersonEndpoints.Map(api);
        //PersonContactEndpoints.Map(api);
        //ResourceAssignmentEndpoints.Map(api);
        //InventoryEndpoints.Map(api);

        return app;
    }
}
