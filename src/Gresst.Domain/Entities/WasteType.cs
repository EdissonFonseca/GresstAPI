using System.Runtime.CompilerServices;
using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

public class WasteType : BaseEntity
{
    public string? Description { get; set; }
    public string? UNCode { get; set; }    
    public string? LERCode { get; set; }
    public string? YCode { get; set; }
    public string? ACode { get; set; }
    public string? ProprietaryCode { get; set; }
    public decimal? KgPerUnit { get; set; }
    public decimal? M3PerUnit { get; set; }
    public decimal? KgPerM3 { get; set; }
    public MeasurementType BaseMeasurementType { get; set; }
    public List<WastePricing> Pricing { get; set; } = new List<WastePricing>();
    public WasteClassification? WasteClassification { get; set; }
    public ICollection<Procedure> CompatibleProcedures { get; set; } = new List<Procedure>();
}