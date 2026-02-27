using Gresst.GraphQL.RouteProcesses;

namespace Gresst.API.GraphQL;

public static class SchemaExtensions
{
    public static IServiceCollection AddGresstGraphQL(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddTypeExtension<TransportQuery>()
            .AddMutationType<RouteProcessMutations>()
            .AddSubscriptionType<RouteProcessSubscriptions>()
            .AddType<FacilityType>()
            .AddInMemorySubscriptions()
            .AddAuthorization()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true);

        return services;
    }
}
