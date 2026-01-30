using Microsoft.AspNetCore.Routing;

namespace Gresst.API.Endpoints;

/// <summary>
/// Registers all Minimal API endpoints under /api/v1.
/// </summary>
public static class MapEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/v1")
            .WithTags("v1");

        AuthenticationEndpoints.Map(api);
        AuthorizationEndpoints.Map(api);
        ClientEndpoints.Map(api);
        FacilityEndpoints.Map(api);
        WasteEndpoints.Map(api);
        ManagementEndpoints.Map(api);
        MaterialEndpoints.Map(api);
        VehicleEndpoints.Map(api);
        PackagingEndpoints.Map(api);
        SupplyEndpoints.Map(api);
        ServiceEndpoints.Map(api);
        WasteClassEndpoints.Map(api);
        TreatmentEndpoints.Map(api);
        RouteEndpoints.Map(api);
        OrderEndpoints.Map(api);
        RequestEndpoints.Map(api);
        ProcessEndpoints.Map(api);
        UserEndpoints.Map(api);
        PersonEndpoints.Map(api);
        PersonContactEndpoints.Map(api);
        ResourceAssignmentEndpoints.Map(api);
        InventoryEndpoints.Map(api);
        MaterialesEndpoints.Map(api);

        return app;
    }
}
