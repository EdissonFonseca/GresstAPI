namespace Gresst.Application.Constants;

/// <summary>
/// Role names used for API authorization. Roles are stored per user (e.g. in Usuario.DatosAdicionales)
/// and included in the JWT at login so RequireRole / policies work.
/// </summary>
/// <remarks>
/// Gresst role hierarchy (account-scoped):
/// - Admin: Full account management (users, authorizations, settings). Can see all account data.
/// - User: Standard operator; create/edit operational data (facilities, routes, orders) within account scope.
/// - Viewer: Read-only access to reports and data within the account.
/// </remarks>
public static class ApiRoles
{
    /// <summary>Account administrator. Can manage users, permissions, and all account data.</summary>
    public const string Admin = "Admin";

    /// <summary>Standard user. Can create and edit operational data within the account.</summary>
    public const string User = "User";

    /// <summary>Read-only. Can view reports and data within the account.</summary>
    public const string Viewer = "Viewer";

    /// <summary>Default role assigned when registering a new user under an existing account.</summary>
    public const string DefaultRole = User;

    /// <summary>Role assigned to the first user when registering a new account.</summary>
    public const string AccountAdminRole = Admin;

    /// <summary>Authorization policy name: only users with Admin role.</summary>
    public const string PolicyAdminOnly = "AdminOnly";

    /// <summary>Authorization policy name: Admin or User (operational access).</summary>
    public const string PolicyAdminOrUser = "AdminOrUser";

    /// <summary>Returns true if the given roles array contains Admin (case-insensitive).</summary>
    public static bool IsAdmin(string[]? roles) =>
        roles != null && roles.Contains(Admin, StringComparer.OrdinalIgnoreCase);

    /// <summary>Returns true if the user has at least User-level access (Admin or User).</summary>
    public static bool IsAdminOrUser(string[]? roles) =>
        roles != null && (roles.Contains(Admin, StringComparer.OrdinalIgnoreCase) || roles.Contains(User, StringComparer.OrdinalIgnoreCase));
}
