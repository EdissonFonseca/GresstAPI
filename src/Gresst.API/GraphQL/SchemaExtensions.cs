using Gresst.GraphQL.RouteProcesses;

namespace Gresst.API.GraphQL;

public static class SchemaExtensions
{
    public static IServiceCollection AddGresstGraphQL(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddQueryType<PartiesQuery>()
            .AddTypeExtension<TransportQuery>()
            .AddMutationType<RouteProcessMutations>()
            .AddSubscriptionType<RouteProcessSubscriptions>()
            .AddType<FacilityType>()
            .AddType<PartyRelatedType>()
            .AddInMemorySubscriptions()
            .AddAuthorization()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true);

        return services;
    }
}
