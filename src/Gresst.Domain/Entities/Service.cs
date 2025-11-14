using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Service types available in the system
/// Examples: Transport, Disposal, Storage, Treatment, Collection, etc.
/// </summary>
public class Service : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    /// <summary>
    /// Service category code
    /// </summary>
    public string? CategoryCode { get; set; }
    
    // Navigation properties
    public virtual ICollection<PersonService> Persons { get; set; } = new List<PersonService>();
    public virtual ICollection<Management> Managements { get; set; } = new List<Management>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}

