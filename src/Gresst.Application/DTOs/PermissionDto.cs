namespace Gresst.Application.DTOs;

/// <summary>
/// DTO for system options/modules
/// </summary>
public class OptionDto
{
    public string Id { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? Description { get; set; }
    public bool IsConfigurable { get; set; }
    public string? Settings { get; set; }
}

/// <summary>
/// DTO for user permissions on options
/// </summary>
public class UserPermissionDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string OptionId { get; set; } = string.Empty;
    public string OptionDescription { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public PermissionFlags Permissions { get; set; }
    public string? Settings { get; set; }
}

/// <summary>
/// DTO to assign permissions to a user
/// </summary>
public class AssignPermissionDto
{
    public Guid UserId { get; set; }
    public string OptionId { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public PermissionFlags Permissions { get; set; }
    public string? Settings { get; set; }
}

/// <summary>
/// Permission flags (C=Create, R=Read, U=Update, D=Delete)
/// </summary>
[Flags]
public enum PermissionFlags
{
    None = 0,
    Create = 1,   // C
    Read = 2,     // R
    Update = 4,   // U
    Delete = 8,   // D
    All = Create | Read | Update | Delete
}

/// <summary>
/// Helper to convert permission string to flags
/// </summary>
public static class PermissionHelper
{
    public static PermissionFlags Parse(string? permissions)
    {
        if (string.IsNullOrEmpty(permissions))
            return PermissionFlags.None;

        var flags = PermissionFlags.None;
        
        if (permissions.Contains('C', StringComparison.OrdinalIgnoreCase))
            flags |= PermissionFlags.Create;
        if (permissions.Contains('R', StringComparison.OrdinalIgnoreCase))
            flags |= PermissionFlags.Read;
        if (permissions.Contains('U', StringComparison.OrdinalIgnoreCase))
            flags |= PermissionFlags.Update;
        if (permissions.Contains('D', StringComparison.OrdinalIgnoreCase))
            flags |= PermissionFlags.Delete;

        return flags;
    }

    public static string ToString(PermissionFlags flags)
    {
        var result = "";
        if (flags.HasFlag(PermissionFlags.Create)) result += "C";
        if (flags.HasFlag(PermissionFlags.Read)) result += "R";
        if (flags.HasFlag(PermissionFlags.Update)) result += "U";
        if (flags.HasFlag(PermissionFlags.Delete)) result += "D";
        return result;
    }

    public static bool HasPermission(string? userPermissions, PermissionFlags requiredPermission)
    {
        var flags = Parse(userPermissions);
        return flags.HasFlag(requiredPermission);
    }
}

