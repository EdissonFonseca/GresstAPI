using Gresst.Domain.Common;

public class TransformationOutput : BaseEntity
{
    public string? FormulaId { get; set; }
    public string? OutputWasteTypeId { get; set; }   // qué tipo de residuo produce
    public decimal? EstimatedPercentage { get; set; } // % del input original
    public MeasurementUnit Unit { get; set; }
}
 