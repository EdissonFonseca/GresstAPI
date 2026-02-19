using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

public class Person : BaseEntity
{
    public string? DocumentNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    
    // Capabilities - What services this person can provide
    public bool IsGenerator { get; set; }
    public bool IsCollector { get; set; }
    public bool IsTransporter { get; set; }
    public bool IsReceiver { get; set; }
    public bool IsDisposer { get; set; }
    public bool IsTreater { get; set; }
    public bool IsStorageProvider { get; set; }
    
}

