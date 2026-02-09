using System.Text.RegularExpressions;

namespace Gresst.Application.Validation;

/// <summary>
/// Password policy validation: no spaces, length 8–128, no repeated-only,
/// at least one upper, lower, digit, and special character. Aligned with
/// OWASP/NIST-style guidance and DoS protection (max length).
/// </summary>
public static class PasswordValidator
{
    public const int MinLength = 8;
    public const int MaxLength = 128;

    private static readonly Regex NoSpaces = new(@"^\S*$", RegexOptions.Compiled);
    private static readonly Regex NoRepeatedOnly = new(@"^(.)\1+$", RegexOptions.Compiled);
    private static readonly Regex HasUppercase = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex HasLowercase = new(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex HasDigit = new(@"\d", RegexOptions.Compiled);
    private static readonly Regex HasSpecial = new(@"[!@#$%^&*(),.?"":{}|<>]", RegexOptions.Compiled);

    public static readonly IReadOnlyList<(string Id, string Message)> RequirementSpecs = new[]
    {
        ("minLength", $"Minimum {MinLength} characters"),
        ("maxLength", $"Maximum {MaxLength} characters (DoS protection)"),
        ("noSpaces", "No spaces allowed"),
        ("noRepeatedOnly", "Cannot be only repeated characters (e.g. aaa)"),
        ("hasUppercase", "At least one uppercase letter (A-Z)"),
        ("hasLowercase", "At least one lowercase letter (a-z)"),
        ("hasDigit", "At least one number (0-9)"),
        ("hasSpecial", "At least one special character (!@#$%^&*(),.?\":{}|<>)")
    };

    /// <summary>
    /// Validates the password and returns per-requirement status and strength.
    /// </summary>
    public static PasswordValidationResult Validate(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return new PasswordValidationResult
            {
                IsValid = false,
                Strength = "weak",
                Requirements = RequirementSpecs
                    .Select(s => new PasswordRequirementResult { Id = s.Id, Message = s.Message, Met = false })
                    .ToList()
            };
        }

        var minLen = password.Length >= MinLength;
        var maxLen = password.Length <= MaxLength;
        var noSpaces = NoSpaces.IsMatch(password);
        var noRepeated = !NoRepeatedOnly.IsMatch(password);
        var hasUpper = HasUppercase.IsMatch(password);
        var hasLower = HasLowercase.IsMatch(password);
        var hasDigit = HasDigit.IsMatch(password);
        var hasSpecialChar = HasSpecial.IsMatch(password);

        var requirements = new List<PasswordRequirementResult>
        {
            new() { Id = "minLength", Message = RequirementSpecs[0].Message, Met = minLen },
            new() { Id = "maxLength", Message = RequirementSpecs[1].Message, Met = maxLen },
            new() { Id = "noSpaces", Message = RequirementSpecs[2].Message, Met = noSpaces },
            new() { Id = "noRepeatedOnly", Message = RequirementSpecs[3].Message, Met = noRepeated },
            new() { Id = "hasUppercase", Message = RequirementSpecs[4].Message, Met = hasUpper },
            new() { Id = "hasLowercase", Message = RequirementSpecs[5].Message, Met = hasLower },
            new() { Id = "hasDigit", Message = RequirementSpecs[6].Message, Met = hasDigit },
            new() { Id = "hasSpecial", Message = RequirementSpecs[7].Message, Met = hasSpecialChar }
        };

        var allMet = minLen && maxLen && noSpaces && noRepeated && hasUpper && hasLower && hasDigit && hasSpecialChar;
        var strength = allMet
            ? (password.Length >= 12 ? "strong" : "medium")
            : "weak";

        return new PasswordValidationResult
        {
            IsValid = allMet,
            Strength = strength,
            Requirements = requirements
        };
    }
}
