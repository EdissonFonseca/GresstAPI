namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Base interface for mapping between Domain entities and Database entities
/// </summary>
/// <typeparam name="TDomain">Domain entity type (English)</typeparam>
/// <typeparam name="TDatabase">Database entity type (Spanish/scaffolded)</typeparam>
public interface IMapper<TDomain, TDatabase>
{
    /// <summary>
    /// Maps from Database entity to Domain entity
    /// </summary>
    TDomain ToDomain(TDatabase dbEntity);
    
    /// <summary>
    /// Maps from Domain entity to Database entity
    /// </summary>
    TDatabase ToDatabase(TDomain domainEntity);
    
    /// <summary>
    /// Updates an existing Database entity with values from Domain entity
    /// </summary>
    void UpdateDatabase(TDomain domainEntity, TDatabase dbEntity);
}

