namespace Gresst.Infrastructure.Common;

/// <summary>
/// Utility class for converting between Guid (Domain) and long (Database)
/// </summary>
public static class GuidLongConverter
{
    /// <summary>
    /// Convert long (database ID) to Guid (domain ID)
    /// </summary>
    /// <param name="id">Database long ID</param>
    /// <returns>Domain Guid ID</returns>
    public static Guid ToGuid(long id)
    {
        if (id == 0) return Guid.Empty;
        return new Guid(id.ToString().PadLeft(32, '0'));
    }

    /// <summary>
    /// Convert Guid (domain ID) to long (database ID)
    /// </summary>
    /// <param name="guid">Domain Guid ID</param>
    /// <returns>Database long ID</returns>
    public static long ToLong(Guid guid)
    {
        if (guid == Guid.Empty) return 0;
        
        var guidString = guid.ToString().Replace("-", "");
        var numericPart = new string(guidString.Where(char.IsDigit).Take(18).ToArray());
        
        return long.TryParse(numericPart, out var result) ? result : 0;
    }

    /// <summary>
    /// Convert string (database ID) to Guid (domain ID)
    /// Handles various formats: numeric strings, guid strings, etc.
    /// </summary>
    /// <param name="id">String ID from database</param>
    /// <returns>Domain Guid ID</returns>
    public static Guid StringToGuid(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return Guid.Empty;

        // Try to parse as Guid directly
        if (Guid.TryParse(id, out var guid))
            return guid;

        // Try to parse as long first
        if (long.TryParse(id, out var longId))
            return ToGuid(longId);

        // Extract only digits and pad to 32 characters
        var numericChars = new string(id.Where(char.IsDigit).ToArray());
        if (string.IsNullOrEmpty(numericChars))
            return Guid.Empty;

        var paddedString = numericChars.PadLeft(32, '0');
        if (paddedString.Length > 32)
            paddedString = paddedString.Substring(0, 32);

        return new Guid(paddedString);
    }

    /// <summary>
    /// Convert Guid (domain ID) to string (database ID)
    /// </summary>
    /// <param name="guid">Domain Guid ID</param>
    /// <returns>String ID for database</returns>
    public static string GuidToString(Guid guid)
    {
        if (guid == Guid.Empty)
            return string.Empty;

        return guid.ToString();
    }

    /// <summary>
    /// Convert nullable long to nullable Guid
    /// </summary>
    public static Guid? ToGuidNullable(long? id)
    {
        return id.HasValue ? ToGuid(id.Value) : null;
    }

    /// <summary>
    /// Convert nullable Guid to nullable long
    /// </summary>
    public static long? ToLongNullable(Guid? guid)
    {
        return guid.HasValue ? ToLong(guid.Value) : null;
    }
}

