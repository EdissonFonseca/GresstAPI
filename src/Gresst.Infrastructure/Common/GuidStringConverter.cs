namespace Gresst.Infrastructure.Common;

/// <summary>
/// Utility class for converting between Guid (Domain) and long (Database)
/// </summary>
public static class GuidStringConverter
{
    /// <summary>
    /// Convert string (database ID) to Guid (domain ID)
    /// </summary>
    /// <param name="id">Database string ID</param>
    /// <returns>Domain Guid ID</returns>
    public static Guid ToGuid(string? id)
    {
        if (String.IsNullOrEmpty(id)) return Guid.Empty;

        var hex = id.PadLeft(32, '0');
        return Guid.ParseExact(hex, "N");
    }

    /// <summary>
    /// Convert Guid (domain ID) to long (database ID)
    /// </summary>
    /// <param name="guid">Domain Guid ID</param>
    /// <returns>Database long ID</returns>
    public static string ToString(Guid guid)
    {
        if (guid == Guid.Empty)
            return null;

        var hex = guid.ToString("N"); // 32 caracteres hex sin guiones

        // Quitamos ceros a la izquierda para obtener el número original
        var trimmed = hex.TrimStart('0');

        return string.IsNullOrEmpty(trimmed) ? "0" : trimmed;
    }

    public static Guid? ToGuidNullable(string? id)
    {
        var result = ToGuid(id);
        return result == Guid.Empty ? null : result;
    }
    public static string? ToStringNullable(Guid? guid)
    {
        return guid.HasValue ? ToString(guid.Value) : null;
    }
}

