using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Person and Treatment
/// Represents treatments that a person (account person, provider) can perform/manage
/// </summary>
public class PersonTreatment : BaseEntity
{
    /// <summary>
    /// Person who can perform/manage this treatment (account person, provider)
    /// </summary>
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    
    /// <summary>
    /// Treatment that the person can perform/manage
    /// </summary>
    public Guid TreatmentId { get; set; }
    public virtual Treatment Treatment { get; set; } = null!;
    
    /// <summary>
    /// Whether the person manages this treatment
    /// </summary>
    public bool IsManaged { get; set; } = true;
    
    /// <summary>
    /// Whether the treatment can be transferred
    /// </summary>
    public bool CanTransfer { get; set; } = false;
}

