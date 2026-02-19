using Gresst.Domain.Common;
using Gresst.Domain.Entities;

public class TransformationFormula : BaseEntity
{
    public string? WasteTypeId { get; set; }         // residuo de entrada
    public string? ProcedureId { get; set; }         // procedimiento aplicado
    public virtual Procedure? Procedure { get; set; }
    public bool IsDefault { get; set; }

    public ICollection<TransformationInput> Inputs { get; set; } = new List<TransformationInput>();
    public ICollection<TransformationOutput> Outputs { get; set; } = new List<TransformationOutput>();// residuos resultantes
}
