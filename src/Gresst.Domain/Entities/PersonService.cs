using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Person and Service
/// Represents services that a person (account person, provider) can provide
/// Examples: Transport, Disposal, Storage, Treatment, Collection, etc.
/// </summary>
public class PersonService : BaseEntity
{
    /// <summary>
    /// Person who provides this service (account person, provider)
    /// </summary>
    public string PersonId { get; set; } = string.Empty;
    public virtual Person Person { get; set; } = null!;
    
    /// <summary>
    /// Service that the person can provide
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;
    public virtual Service Service { get; set; } = null!;
    
    /// <summary>
    /// Start date when the person can provide this service
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// End date when the person stops providing this service (null = indefinite)
    /// </summary>
    public DateTime? EndDate { get; set; }
}

