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
        return Guid.ParseExact(id.ToString().PadLeft(32, '0'), "N");
    }

    /// <summary>
    /// Convert Guid (domain ID) to long (database ID)
    /// </summary>
    /// <param name="guid">Domain Guid ID</param>
    /// <returns>Database long ID</returns>
    public static long ToLong(Guid guid)
    {
        if (guid == Guid.Empty) return 0;

        var digits = new string(guid.ToString("N").Where(char.IsDigit).ToArray());
        var last18 = digits.Substring(Math.Max(0, digits.Length - 18));

        return long.TryParse(last18, out var result) ? result : 0;
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

