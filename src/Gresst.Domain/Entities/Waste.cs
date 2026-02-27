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
    public string? YCode { get; set; }
    public string? ACode { get; set; }
    public string? ProprietaryCode { get; set; }
    public decimal? KgPerItem { get; set; }
    public decimal? M3PerItem { get; set; }
    public decimal? KgPerM3 { get; set; }
    public MeasurementType BaseMeasurementType { get; set; }
    public decimal? PurchasePrice { get; set; }     
    public decimal? SalePrice { get; set; }
    public decimal? CollectionPrice { get; set; }
    public decimal? TreatmentPrice { get; set; }
    public decimal? StoragePrice { get; set; }
    public decimal? TransportPrice { get; set; }
    public WasteClassification? WasteClassification { get; set; }
    public ICollection<Procedure> CompatibleProcedures { get; set; } = new List<Procedure>();
}