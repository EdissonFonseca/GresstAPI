using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Waste is the definition of a type of waste, with its properties and compatible procedures. It is used to classify waste items and determine how they should be handled.
/// </summary>
public class Waste : BaseEntity
{
    public string? Description { get; set; }
    public string? UNCode { get; set; }    
    public string? LERCode { get; set; }
    public string? BaselYCode { get; set; }
    public string? BaselACode { get; set; }
    public decimal? KgPerItem { get; set; }
    public decimal? M3PerItem { get; set; }
    public decimal? KgPerM3 { get; set; }
    public WasteType WasteType { get; set; }
    public MeasurementType BaseMeasurementType { get; set; }
    public ICollection<Procedure> CompatibleProcedures { get; set; } = new List<Procedure>();
}