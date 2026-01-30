namespace Gresst.Infrastructure.Common;

/// <summary>
/// Converts between string (domain ID) and long (database ID).
/// </summary>
public static class IdConversion
{
    /// <summary>
    /// Converts string (domain ID, numeric or Guid format) to long (database ID).
    /// </summary>
    public static long ToLongFromString(string? id)
    {
        if (string.IsNullOrEmpty(id)) return 0;
        if (long.TryParse(id, out var l)) return l;
        if (Guid.TryParse(id, out var g))
        {
            var digits = new string(g.ToString("N").Where(char.IsDigit).ToArray());
            var last18 = digits.Length <= 18 ? digits : digits.Substring(digits.Length - 18);
            return long.TryParse(last18, out var result) ? result : 0;
        }
        return 0;
    }

    /// <summary>
    /// Converts long (database ID) to string (domain ID).
    /// </summary>
    public static string ToStringFromLong(long id)
    {
        return id.ToString();
    }
}
