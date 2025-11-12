namespace Gresst.Domain.Enums;

/// <summary>
/// Status of an account
/// </summary>
public enum AccountStatus
{
    /// <summary>
    /// Active account (IdEstado = "A" in database)
    /// </summary>
    Active,
    
    /// <summary>
    /// Inactive account (IdEstado = "I" in database)
    /// </summary>
    Inactive,
    
    /// <summary>
    /// Suspended account (IdEstado = "S" in database)
    /// </summary>
    Suspended,
    
    /// <summary>
    /// Blocked account (IdEstado = "B" in database)
    /// </summary>
    Blocked
}

