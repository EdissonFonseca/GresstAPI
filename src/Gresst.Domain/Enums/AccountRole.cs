namespace Gresst.Domain.Enums;

/// <summary>
/// Role of an account in waste management
/// </summary>
public enum AccountRole
{
    /// <summary>
    /// Waste generator (IdRol = "N" in database)
    /// </summary>
    Generator,
    
    /// <summary>
    /// Logistic operator - collects, transports, treats waste (IdRol = "S" in database)
    /// </summary>
    Operator,
    
    /// <summary>
    /// Both generator and operator
    /// </summary>
    Both
}

