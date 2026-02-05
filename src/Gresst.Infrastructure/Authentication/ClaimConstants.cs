namespace Gresst.Infrastructure.Authentication;

/// <summary>
/// Claim names and values used to differentiate human vs service authentication.
/// </summary>
public static class ClaimConstants
{
    /// <summary>
    /// Claim that indicates the type of principal: "human" (interactive user) or "service" (machine/client).
    /// </summary>
    public const string SubjectType = "subject_type";

    /// <summary>
    /// Value for interactive users (login with username/password or external provider).
    /// </summary>
    public const string SubjectTypeHuman = "human";

    /// <summary>
    /// Value for service/machine clients (token obtained via interface + client token).
    /// </summary>
    public const string SubjectTypeService = "service";

    /// <summary>
    /// Claim that indicates which interface/client obtained the token (only set for service tokens).
    /// </summary>
    public const string Interface = "interface";

    /// <summary>OAuth 2.0 client identifier (service/client).</summary>
    public const string ClientId = "client_id";

    /// <summary>OAuth 2.0 scope claim (space-separated list of permissions).</summary>
    public const string Scope = "scope";
}
