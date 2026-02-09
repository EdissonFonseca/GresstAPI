namespace Gresst.Application.Validation;

/// <summary>
/// Result of password validation for API responses. Enables clients to show
/// requirement checkmarks and a strength indicator (weak / medium / strong).
/// </summary>
public class PasswordValidationResult
{
    public bool IsValid { get; set; }
    /// <summary>Strength for UI: "weak" (red), "medium" (yellow), "strong" (green).</summary>
    public string Strength { get; set; } = "weak";
    public IReadOnlyList<PasswordRequirementResult> Requirements { get; set; } = Array.Empty<PasswordRequirementResult>();
}

public class PasswordRequirementResult
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Met { get; set; }
}
