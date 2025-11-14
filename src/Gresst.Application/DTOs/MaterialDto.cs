namespace Gresst.Application.DTOs;

public class MaterialDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Synonyms { get; set; }
    public string? Reference { get; set; }
    public Guid? WasteTypeId { get; set; }
    public string? WasteTypeName { get; set; }
    public string? Image { get; set; }
    public string? Measurement { get; set; } // P = Peso, V = Volumen
    public bool IsPublic { get; set; }
    public bool IsRecyclable { get; set; }
    public bool IsHazardous { get; set; }
    public string? Category { get; set; } // Metal, Plastic, Glass, Organic, etc.
    
    // Pricing
    public decimal? ServicePrice { get; set; }
    public decimal? PurchasePrice { get; set; }
    
    // Physical properties
    public decimal? Weight { get; set; }
    public decimal? Volume { get; set; }
    public decimal? EmissionCompensationFactor { get; set; }
    
    public bool IsActive { get; set; }
    public bool IsReusable { get; set; } // Aprovechable
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateMaterialDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Synonyms { get; set; }
    public string? Reference { get; set; }
    public Guid? WasteTypeId { get; set; }
    public string? Image { get; set; }
    public string? Measurement { get; set; } // P = Peso, V = Volumen
    public bool IsPublic { get; set; } = true;
    public bool IsRecyclable { get; set; }
    public bool IsHazardous { get; set; }
    public string? Category { get; set; }
    
    // Pricing
    public decimal? ServicePrice { get; set; }
    public decimal? PurchasePrice { get; set; }
    
    // Physical properties
    public decimal? Weight { get; set; }
    public decimal? Volume { get; set; }
    public decimal? EmissionCompensationFactor { get; set; }
    
    public bool IsReusable { get; set; }
}

public class UpdateMaterialDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Synonyms { get; set; }
    public string? Reference { get; set; }
    public Guid? WasteTypeId { get; set; }
    public string? Image { get; set; }
    public string? Measurement { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsRecyclable { get; set; }
    public bool? IsHazardous { get; set; }
    public string? Category { get; set; }
    
    // Pricing
    public decimal? ServicePrice { get; set; }
    public decimal? PurchasePrice { get; set; }
    
    // Physical properties
    public decimal? Weight { get; set; }
    public decimal? Volume { get; set; }
    public decimal? EmissionCompensationFactor { get; set; }
    
    public bool? IsActive { get; set; }
    public bool? IsReusable { get; set; }
}

