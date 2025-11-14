using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Materials for transformation/decomposition
/// Represents how a material can be decomposed into other materials or transformed into another material
/// Uses conversion factors (percentage, weight, quantity, volume)
/// </summary>
public class MaterialTransformation : BaseEntity
{
    /// <summary>
    /// Source material (the material being decomposed or transformed)
    /// </summary>
    public Guid SourceMaterialId { get; set; }
    public virtual Material SourceMaterial { get; set; } = null!;
    
    /// <summary>
    /// Result material (the material resulting from decomposition or transformation)
    /// </summary>
    public Guid ResultMaterialId { get; set; }
    public virtual Material ResultMaterial { get; set; } = null!;
    
    /// <summary>
    /// Relationship type code (e.g., "DE" for Decomposition, "TR" for Transformation, "CO" for Composition)
    /// </summary>
    public string RelationshipType { get; set; } = string.Empty;
    
    /// <summary>
    /// Conversion factor as percentage (0-100)
    /// </summary>
    public decimal? Percentage { get; set; }
    
    /// <summary>
    /// Conversion factor as quantity (units)
    /// </summary>
    public decimal? Quantity { get; set; }
    
    /// <summary>
    /// Conversion factor as weight (kg, tons, etc.)
    /// </summary>
    public decimal? Weight { get; set; }
    
    /// <summary>
    /// Conversion factor as volume (m3, liters, etc.)
    /// </summary>
    public decimal? Volume { get; set; }
    
    // Note: IsActive is inherited from BaseEntity
}

