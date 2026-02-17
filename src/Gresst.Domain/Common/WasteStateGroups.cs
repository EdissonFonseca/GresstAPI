using Gresst.Domain.Enums;

namespace Gresst.Domain.Common;

/// <summary>
/// Groups waste lifecycle states into pre-reception vs post-reception.
/// States answer "where is the waste"; business rules use this to determine allowed actions.
/// </summary>
public static class WasteStateGroups
{
    public const string PreReception = "PreReception";
    public const string PostReception = "PostReception";

    /// <summary>
    /// Returns the group for the given waste status.
    /// </summary>
    public static string GetGroup(WasteStatus status)
    {
        return status switch
        {
            WasteStatus.Generated => PreReception,
            WasteStatus.InTransit => PreReception,
            _ => PostReception
        };
    }

    /// <summary>
    /// True if the waste has not yet been received.
    /// </summary>
    public static bool IsPreReception(WasteStatus status) => GetGroup(status) == PreReception;

    /// <summary>
    /// True if the waste has been received (stored, in treatment, disposed, etc.).
    /// </summary>
    public static bool IsPostReception(WasteStatus status) => GetGroup(status) == PostReception;
}
