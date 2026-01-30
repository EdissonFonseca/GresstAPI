using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Person and Supply
/// Represents supplies that a person (account person) uses in their logistics operations
/// </summary>
public class PersonSupply : BaseEntity
{
    /// <summary>
    /// Person who uses/manages this supply (account person)
    /// </summary>
    public string PersonId { get; set; } = string.Empty;
    public virtual Person Person { get; set; } = null!;
    
    /// <summary>
    /// Supply that the person uses
    /// </summary>
    public string SupplyId { get; set; } = string.Empty;
    public virtual Supply Supply { get; set; } = null!;
    
    /// <summary>
    /// Price for this supply
    /// </summary>
    public decimal? Price { get; set; }
    
    // Note: IsActive is inherited from BaseEntity
}

