using System.Runtime.CompilerServices;
using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

public class WasteType : BaseEntity
{
    public string? Description { get; set; }
    
    // International Classifications
    public string? UNCode { get; set; }    
    public string? LERCode { get; set; }
    public string? YCode { get; set; }
    public string? ACode { get; set; }
    public string? ProprietaryCode { get; set; }
    public ValorizationType ValorizationType { get; set; } // Generator receives money
    public RegulatoryType RegulatoryType { get; set; }
    public PhysicalState? PhysicalState { get; set; } // Solid, Liquid, Gas, Sludge
    public ICollection<Procedure> CompatibleProcedures { get; set; } = new List<Procedure>();
}