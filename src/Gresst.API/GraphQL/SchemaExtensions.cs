namespace Gresst.API.GraphQL;

public static class SchemaExtensions
{
    public static IServiceCollection AddGresstGraphQL(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddQueryType<PartiesQuery>()
            .AddType<FacilityType>()
            .AddType<PartyRelatedType>()
            .AddAuthorization()  // ← agregar esto
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true);

        return services;
    }
}
