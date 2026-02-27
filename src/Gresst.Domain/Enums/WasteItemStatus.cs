public enum WasteItemStatus
{
    /// <summary>
    /// Registered but not yet confirmed.
    /// No operations can be performed until confirmed.
    /// </summary>
    Draft,

    /// <summary>
    /// Confirmed and operational. Has quantity available.
    /// All operations are permitted.
    /// </summary>
    Active,

    /// <summary>
    /// Zero quantity. Exists for traceability only.
    /// Result of Splitting or Merging operations.
    /// Can return to Active only via Adjustment.
    /// </summary>
    Depleted,

    /// <summary>
    /// Permanently exited the system.
    /// Result of Disposal, Valorization or Donation operations.
    /// No further operations permitted.
    /// </summary>
    Disposed,

    /// <summary>
    /// Permanently confined. Will not move again.
    /// Result of Containment operation.
    /// No further operations permitted.
    /// </summary>
    Contained,

    /// <summary>
    /// Physical loss recorded via Adjustment with negative quantity.
    /// Examples: spillage, accidental destruction, theft.
    /// No further operations permitted.
    /// </summary>
    Lost,

    /// <summary>
    /// Blocked by an active incident.
    /// No operations permitted until incident is resolved and residue is manually reactivated.
    /// </summary>
    Incident,

    /// <summary>
    /// Administratively cancelled. Record voided.
    /// No further operations permitted.
    /// </summary>
    Cancelled
}