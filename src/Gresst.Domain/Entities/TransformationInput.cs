using Gresst.Domain.Common;

public class TransformationInput : BaseEntity
{
    public string? FormulaId { get; set; }
    public string? WasteTypeId { get; set; }
    public decimal? EstimatedPercentage { get; set; }
}
