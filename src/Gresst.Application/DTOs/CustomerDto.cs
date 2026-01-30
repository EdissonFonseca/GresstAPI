namespace Gresst.Application.DTOs;

/// <summary>
/// DTO for Customer (Person with Customer role)
/// </summary>
public class CustomerDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string AccountId { get; set; } = string.Empty;

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
    public string IdPersona { get; set; } = string.Empty; // Backward compatibility
    public string Nombre { get; set; } = string.Empty;  // Backward compatibility
}

public class CreateCustomerDto
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

public class UpdateCustomerDto
{
    public string Id { get; set; } = string.Empty;
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
