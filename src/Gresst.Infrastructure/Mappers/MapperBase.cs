namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Base class for mappers with common mapping logic
/// </summary>
public abstract class MapperBase<TDomain, TDatabase> : IMapper<TDomain, TDatabase>
{
    public abstract TDomain ToDomain(TDatabase dbEntity);
    
    public abstract TDatabase ToDatabase(TDomain domainEntity);
    
    public abstract void UpdateDatabase(TDomain domainEntity, TDatabase dbEntity);
    
    /// <summary>
    /// Helper method to map collections from DB to Domain
    /// </summary>
    protected ICollection<TDomainItem> MapCollectionToDomain<TDomainItem, TDatabaseItem>(
        ICollection<TDatabaseItem> dbCollection,
        IMapper<TDomainItem, TDatabaseItem> mapper)
    {
        if (dbCollection == null || dbCollection.Count == 0)
            return new List<TDomainItem>();
            
        return dbCollection.Select(mapper.ToDomain).ToList();
    }
    
    /// <summary>
    /// Helper method to map collections from Domain to DB
    /// </summary>
    protected ICollection<TDatabaseItem> MapCollectionToDatabase<TDomainItem, TDatabaseItem>(
        ICollection<TDomainItem> domainCollection,
        IMapper<TDomainItem, TDatabaseItem> mapper)
    {
        if (domainCollection == null || domainCollection.Count == 0)
            return new List<TDatabaseItem>();
            
        return domainCollection.Select(mapper.ToDatabase).ToList();
    }
}

