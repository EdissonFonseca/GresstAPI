using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Person and Packaging
/// Represents packaging types that a person (account person) manages/handles
/// </summary>
public class PersonPackaging : BaseEntity
{
    /// <summary>
    /// Person who manages this packaging (account person)
    /// </summary>
    public string PersonId { get; set; } = string.Empty;
    public virtual Person Person { get; set; } = null!;
    
    /// <summary>
    /// Packaging type that the person manages
    /// </summary>
    public string PackagingId { get; set; } = string.Empty;
    public virtual Packaging Packaging { get; set; } = null!;
    
    /// <summary>
    /// Price for this packaging
    /// </summary>
    public decimal? Price { get; set; }
}

