using Gresst.Domain.Enums;

namespace Gresst.Application.WasteManagement;

/// <summary>
/// Rules: each activity (collection, reception, disposal, transformation, etc.) can be certified.
/// Certification may cover the full quantity or a partial quantity of the residue.
/// </summary>
public static class WasteActivityCertification
{
    /// <summary>
    /// Activity types that can be certified. Each corresponds to a lifecycle transition or operation.
    /// </summary>
    public enum CertifiableActivity
    {
        Collection = 1,
        Reception = 2,
        Treatment = 3,
        Disposal = 4,
        Transfer = 5,
        Transformation = 6,
        Closure = 7
    }

    /// <summary>
    /// States/transitions that have an associated certifiable activity.
    /// </summary>
    private static readonly Dictionary<WasteLifecycleState, CertifiableActivity> StateToActivity = new()
    {
        [WasteLifecycleState.InTransit] = CertifiableActivity.Collection,
        [WasteLifecycleState.Received] = CertifiableActivity.Reception,
        [WasteLifecycleState.Treated] = CertifiableActivity.Treatment,
        [WasteLifecycleState.Disposed] = CertifiableActivity.Disposal,
        [WasteLifecycleState.Transferred] = CertifiableActivity.Transfer,
        [WasteLifecycleState.Transformed] = CertifiableActivity.Transformation,
        [WasteLifecycleState.Closed] = CertifiableActivity.Closure
    };

    /// <summary>
    /// All activities that support certification.
    /// </summary>
    public static IReadOnlyList<CertifiableActivity> AllCertifiableActivities { get; } =
        Enum.GetValues<CertifiableActivity>().ToList();

    /// <summary>
    /// True if the given activity type supports certification.
    /// </summary>
    public static bool IsCertifiable(CertifiableActivity activity) => true;

    /// <summary>
    /// Returns the certifiable activity associated with reaching this state (if any).
    /// </summary>
    public static CertifiableActivity? GetActivityForState(WasteLifecycleState targetState)
    {
        return StateToActivity.TryGetValue(targetState, out var activity) ? activity : null;
    }

    /// <summary>
    /// True if reaching this target state corresponds to a certifiable activity.
    /// </summary>
    public static bool IsCertifiableState(WasteLifecycleState targetState) =>
        GetActivityForState(targetState).HasValue;
}
