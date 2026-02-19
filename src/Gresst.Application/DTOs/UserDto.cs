namespace Gresst.Application.DTOs;

/// <summary>
/// DTO para información completa del usuario
/// </summary>
public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // A=Active, I=Inactive
    public bool IsActive => Status == "A";
    public string? PartyId { get; set; }
    public string? PartyName { get; set; }
    public string[]? Roles { get; set; }
    public DateTime? LastAccess { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO para crear/actualizar usuario
/// </summary>
public class CreateUserDto
{
    public string AccountId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? PersonId { get; set; }
    public string[]? Roles { get; set; }
}

/// <summary>
/// DTO para actualizar perfil del usuario
/// </summary>
public class UpdateUserProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// DTO para cambiar contraseña
/// </summary>
public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

