namespace Gresst.Application.DTOs;

/// <summary>
/// DTO para Proveedor (Persona con rol de Proveedor)
/// </summary>
public class ProviderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public Guid AccountId { get; set; }
    
    // Capabilities
    public bool IsGenerator { get; set; }
    public bool IsCollector { get; set; }
    public bool IsTransporter { get; set; }
    public bool IsReceiver { get; set; }
    public bool IsDisposer { get; set; }
    public bool IsTreater { get; set; }
    public bool IsStorageProvider { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateProviderDto
{
    public string Name { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    
    // Capabilities
    public bool IsGenerator { get; set; }
    public bool IsCollector { get; set; }
    public bool IsTransporter { get; set; }
    public bool IsReceiver { get; set; }
    public bool IsDisposer { get; set; }
    public bool IsTreater { get; set; }
    public bool IsStorageProvider { get; set; }
}

public class UpdateProviderDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? DocumentNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    
    // Capabilities
    public bool? IsGenerator { get; set; }
    public bool? IsCollector { get; set; }
    public bool? IsTransporter { get; set; }
    public bool? IsReceiver { get; set; }
    public bool? IsDisposer { get; set; }
    public bool? IsTreater { get; set; }
    public bool? IsStorageProvider { get; set; }
    
    public bool? IsActive { get; set; }
}

