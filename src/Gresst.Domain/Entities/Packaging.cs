using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Packaging types for waste containers
/// </summary>
public class Packaging : BaseEntity
{
    public string? Description { get; set; }
    
    // Capacity
    public decimal? Capacity { get; set; }
    public string? CapacityUnit { get; set; }
    
    // Properties
    public bool IsReusable { get; set; }
    
    // UN Packaging codes (for hazardous waste)
    public string? UNPackagingCode { get; set; }
}

