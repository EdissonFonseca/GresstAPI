using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

public class Person : BaseEntity
{
    public string Name { get; set; } = string.Empty;
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
    
    // Navigation properties
    public virtual ICollection<License> Licenses { get; set; } = new List<License>();
    public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public virtual ICollection<Request> RequestsAsRequester { get; set; } = new List<Request>();
    public virtual ICollection<Request> RequestsAsProvider { get; set; } = new List<Request>();
    public virtual ICollection<Balance> Balances { get; set; } = new List<Balance>();
}

