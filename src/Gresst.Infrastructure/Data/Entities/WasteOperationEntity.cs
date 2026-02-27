namespace Gresst.Infrastructure.Data.Entities;

/// <summary>
/// Persistence model for WasteOperation. Data is stored as JSON.
/// </summary>
public class WasteOperationEntity
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid? ProcessId { get; set; }
    public DateTime OccurredAt { get; set; }
    public string DataJson { get; set; } = string.Empty;
}
