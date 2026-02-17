namespace Gresst.Application.WasteManagement;

/// <summary>
/// Rules: each activity (reception, disposal, transformation, treatment, transfer, etc.) can be performed
/// on a partial quantity of the residue. The remainder stays in the current state (or is tracked separately).
/// Certification may also cover only part of the residue.
/// </summary>
public static class WastePartialQuantityRules
{
    /// <summary>
    /// Activities that support partial execution (quantity less than total residue quantity).
    /// When executed partially, the system must track: quantity applied to this activity, quantity remaining.
    /// </summary>
    public const bool PartialAllowedForAllActivities = true;

    /// <summary>
    /// Validates that a partial quantity is valid for the operation.
    /// </summary>
    /// <param name="operationQuantity">Quantity being processed in this operation.</param>
    /// <param name="availableQuantity">Total quantity currently available (e.g. residue quantity).</param>
    /// <param name="allowPartial">If true, operationQuantity may be less than availableQuantity.</param>
    /// <returns>True if valid.</returns>
    public static bool IsValidQuantity(decimal operationQuantity, decimal availableQuantity, bool allowPartial = true)
    {
        if (operationQuantity <= 0) return false;
        if (operationQuantity > availableQuantity) return false;
        return allowPartial || operationQuantity == availableQuantity;
    }

    /// <summary>
    /// When an activity is executed on a partial quantity, the remaining quantity (availableQuantity - operationQuantity)
    /// stays in the current state. The part that was processed transitions (e.g. to Disposed, Transferred).
    /// Implementation: either split into two residue records (one remaining, one transferred/disposed) or track
    /// remaining quantity on the same record and create a child/sibling for the processed part, per domain model.
    /// </summary>
    public const string PartialExecutionRule =
        "Partial quantity: processed part transitions; remainder stays in current state (split or same record per model).";

    /// <summary>
    /// Certification can be issued for a partial quantity of the residue (e.g. certify 80% received, 20% pending).
    /// </summary>
    public const bool CertificationSupportsPartialQuantity = true;
}
