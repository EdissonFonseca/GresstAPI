using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Person and WasteClass
/// Represents waste classes that a person (account person, provider, or client) is associated with
/// </summary>
public class PersonWasteClass : BaseEntity
{
    /// <summary>
    /// Person who is associated with this waste class
    /// </summary>
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    
    /// <summary>
    /// Waste class that the person is associated with
    /// </summary>
    public Guid WasteClassId { get; set; }
    public virtual WasteClass WasteClass { get; set; } = null!;
    
    // Note: IsActive is inherited from BaseEntity
}

