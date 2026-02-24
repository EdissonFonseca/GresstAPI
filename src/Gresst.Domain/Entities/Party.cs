using Gresst.Domain.Common;
using NetTopologySuite.Geometries;

namespace Gresst.Domain.Entities;

public class Party : BaseEntity
{
    public PersonType? PersonType { get; set; }
    public string? DocumentNumber { get; set; }
    public int? CheckDigit { get; set; }    
    public DocumentType? DocumentType { get; set; }

    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Phone2 { get; set; }
    public Point? Location { get; set; }
    public string? LocalityId { get; set; }
    public string? SignatureUrl { get; set; }

    public List<PartyRelationType> Relations { get; set; } = new List<PartyRelationType>();
    public PartyType Type { get; set; }
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
    public ICollection<License> Licenses { get; set; } = new List<License>();
    public ICollection<Packaging> Packaging { get; set; } = new List<Packaging>();
    public ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public ICollection<WasteType> WasteTypes { get; set; } = new List<WasteType>();
    public ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
