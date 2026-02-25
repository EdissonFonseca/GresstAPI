using Gresst.Domain.Common;
using NetTopologySuite.Geometries;

namespace Gresst.Domain.Entities;

/// <summary>
/// Facilities are physical installations like treatment plants, disposal sites, storage facilities
/// </summary>
public class Facility : BaseEntity
{
    public string? ParentId { get; set; }
    public string? LocalityId { get; set; }
    public Point? Location { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Reference { get; set; }
    public List<FacilityType>? Types { get; set; }
    public List<Facility> Facilities { get; set; } = new List<Facility>();    
    public List<WasteType> WasteTypes { get; set; } = new List<WasteType>();
    public List<Procedure> Procedures { get; set; } = new List<Procedure>();
    public List<Contact> Contacts { get; set; } = new List<Contact>();
}

