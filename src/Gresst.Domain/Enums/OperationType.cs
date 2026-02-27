namespace Gresst.Domain.Common;

/// <summary>
/// Atomic operations that can occur on a residue.
/// Each operation affects a specific dimension of the residue.
/// Operations are always contained within at least one Process.
/// </summary>
public enum OperationType
{
    /// <summary>
    /// A residue comes into existence.
    /// Dimension: Existence
    /// Processes: Collection, InSiteStorage
    /// </summary>
    Generation = 1,

    /// <summary>
    /// A residue moves from one physical location to another.
    /// Dimension: Location
    /// Processes: Transport, Processing, Treatment, FinalDisposal, Return
    /// </summary>
    Relocation,

    /// <summary>
    /// Legal custody of a residue changes from one party to another.
    /// The Generator never changes — only the Holder.
    /// Dimension: Ownership
    /// Processes: Transport, Collection, InventoryAdjustment, Return
    /// </summary>
    Transfer,

    /// <summary>
    /// The physical form or presentation of a residue changes,
    /// but its chemical nature remains the same. Reversible.
    /// Examples: shredding, compacting, filtering, cleaning.
    /// Dimension: Physical Form
    /// Processes: Processing, IntegratedTreatment
    /// </summary>
    Processing,

    /// <summary>
    /// The chemical or biological nature of a residue changes fundamentally.
    /// 1 input residue → 1 output residue. Not reversible.
    /// Examples: composting, re-refining.
    /// Dimension: Nature
    /// Processes: Treatment, IntegratedTreatment, BiogasProduction
    /// </summary>
    Transformation,

    /// <summary>
    /// One residue is divided into multiple output residues.
    /// The parent residue remains with zero quantity for traceability (Depleted).
    /// Each child can have a different classification than the parent.
    /// 1 input residue → N output residues.
    /// Examples: segregation of compound residues, WEEE disassembly.
    /// Dimension: Nature
    /// Processes: Segregation, IntegratedTreatment
    /// </summary>
    Splitting,

    /// <summary>
    /// Multiple residues are combined into a single output residue.
    /// Source residues remain with zero quantity for traceability (Depleted).
    /// N input residues → 1 output residue.
    /// Examples: oil mixing before re-refining, glass consolidation.
    /// Dimension: Nature
    /// Processes: Consolidation, Treatment, BiogasProduction
    /// </summary>
    Merging,

    /// <summary>
    /// A residue ceases to exist operationally.
    /// Triggered by final disposal operations.
    /// Dimension: Existence
    /// Processes: FinalDisposal, Incineration
    /// </summary>
    Disposal,

    /// <summary>
    /// A residue is held temporarily in a location.
    /// Operational state is active — the residue will move again.
    /// Examples: temporary storage at facility, vehicle storage during transport.
    /// Dimension: Operational State
    /// Processes: Transport, InSiteStorage, Collection
    /// </summary>
    Storage,

    /// <summary>
    /// A residue is permanently confined in a location.
    /// Operational state is final — the residue will not move again.
    /// Examples: sanitary landfill, security cell.
    /// Dimension: Operational State
    /// Processes: PermanentStorage
    /// </summary>
    Containment,

    /// <summary>
    /// The recorded quantity of a residue is corrected.
    /// The only operation that can reactivate a Depleted residue.
    /// Examples: physical count correction, accidental loss, spillage.
    /// Dimension: Quantity
    /// Processes: InventoryAdjustment
    /// </summary>
    Adjustment,

    /// <summary>
    /// A residue exits the system as a product with commercial value or free of charge.
    /// The residue transitions to Disposed status.
    /// The resulting product enters the residue marketplace.
    /// Examples: recycled PET pellets, compost, re-refined oil.
    /// Dimension: Existence
    /// Processes: Valorization, BiogasProduction, IntegratedTreatment
    /// </summary>
    Valorization,

    /// <summary>
    /// A residue exits the system without commercial value,
    /// transferred directly to a recipient organization.
    /// The residue transitions to Disposed status.
    /// Applies to any residue type.
    /// Examples: used clothing to NGOs, functional electronics to schools.
    /// Dimension: Existence
    /// Processes: Donation
    /// </summary>
    Donation
}
