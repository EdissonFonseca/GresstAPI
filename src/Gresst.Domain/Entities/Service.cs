using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Service types available in the system
/// Examples: Transport, Disposal, Storage, Treatment, Collection, etc.
/// </summary>
public class Service : BaseEntity
{
    public Procedure? Procedure { get; set; }
    public ServiceDeliveryType ServiceDeliveryType { get; set; }
    public Money? Price { get; set; }
    public string? Notes { get; set; }
    public List<string>? PartyIds { get; set; } = new List<string>();
}

