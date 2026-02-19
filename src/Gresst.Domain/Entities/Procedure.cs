using Gresst.Domain.Common;

public class Procedure : BaseEntity
{
    public string ProcedureId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public OperationType OperationType { get; set; }
}