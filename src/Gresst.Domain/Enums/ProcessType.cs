namespace Gresst.Domain.Common;

/// <summary>
/// Composite processes that orchestrate one or more Operations on residues.
/// The steps documented for each process represent the typical flow.
/// Actual implementations may include additional steps depending on context
/// (e.g., classification at origin, intermediate inspections).
/// </summary>
public enum ProcessType
{
    /// <summary>
    /// Physical movement of residues between locations under legal custody chain.
    /// Requires a transport manifest that collects signatures at each step.
    ///
    /// Typical steps:
    ///   1. Transfer    — generator delivers residue to transporter (custody in, if applicable)
    ///   2. Relocation  — residue moves from generator site to vehicle
    ///   3. Storage     — temporary storage on vehicle during route (optional)
    ///   4. Relocation  — residue moves from vehicle to destination site
    ///   5. Transfer    — transporter delivers to receiver (custody out, if applicable)
    /// </summary>
    Transport = 1,

    /// <summary>
    /// Pickup of residues from multiple generation points into a collection vehicle.
    /// Common in urban/municipal waste management.
    /// No formal Transfer required at each stop — custody passes implicitly.
    ///
    /// Typical steps:
    ///   1. Generation  — residues exist at generation points
    ///   2. Relocation  — residues loaded onto collection vehicle at each stop
    ///   3. Storage     — temporary accumulation on vehicle during route
    ///   4. Relocation  — residues unloaded at reception facility
    ///   5. Transfer    — custody transferred to facility operator
    /// </summary>
    Collection,

    /// <summary>
    /// Physical transformation of residues without changing their chemical nature.
    /// Changes the form, size, or physical state of the residue.
    /// Examples: shredding, compacting, filtering, cleaning, drying.
    ///
    /// Typical steps:
    ///   1. Relocation  — residue enters processing facility
    ///   2. Processing  — physical form changes
    ///   3. Relocation  — processed residue exits to storage or next process
    /// </summary>
    Processing,

    /// <summary>
    /// Chemical or biological transformation of residues, changing their nature.
    /// 1 input residue produces 1 output residue of different nature.
    /// Examples: incineration, composting, chemical neutralization,
    ///           oil re-refining, tire retreading.
    ///
    /// Typical steps:
    ///   1. Relocation     — residue enters treatment facility
    ///   2. Transformation — nature of residue changes
    ///   3. Valorization   — output product enters marketplace (if applicable)
    ///   4. Relocation     — treated residue exits to storage or disposal
    /// </summary>
    Treatment,

    /// <summary>
    /// Division of a compound residue into multiple component residues.
    /// Parent residue remains with zero quantity (Depleted) for traceability.
    /// Each child residue can have different classification than parent.
    /// Examples: WEEE disassembly, hospital waste segregation by hazard level,
    ///           separation of construction waste components.
    ///
    /// Typical steps:
    ///   1. Relocation  — residue enters segregation facility
    ///   2. Splitting   — residue divided into classified components
    ///   3. Relocation  — each component routed to appropriate next process
    /// </summary>
    Segregation,

    /// <summary>
    /// Combination of multiple compatible residues into a single residue.
    /// Source residues remain with zero quantity (Depleted) for traceability.
    /// Examples: used oil consolidation before re-refining,
    ///           glass consolidation before recycling.
    ///
    /// Typical steps:
    ///   1. Relocation  — residues arrive at consolidation facility
    ///   2. Merging     — compatible residues combined into one
    ///   3. Relocation  — consolidated residue routed to treatment or storage
    /// </summary>
    Consolidation,

    /// <summary>
    /// Permanent elimination of residues at an authorized disposal site.
    /// The residue ceases to exist operationally after this process.
    /// Examples: sanitary landfill, controlled incineration without recovery.
    ///
    /// Typical steps:
    ///   1. Relocation  — residue transported to disposal site
    ///   2. Disposal    — residue permanently eliminated
    /// </summary>
    FinalDisposal,

    /// <summary>
    /// Permanent confinement of residues that cannot be treated or disposed.
    /// The residue remains physically but is operationally closed.
    /// Examples: security cells for hazardous residues, nuclear containment (excluded).
    ///
    /// Typical steps:
    ///   1. Relocation   — residue transported to containment facility
    ///   2. Containment  — residue permanently confined
    /// </summary>
    PermanentStorage,

    /// <summary>
    /// Correction of recorded residue quantities.
    /// The only process that can reactivate a Depleted residue.
    /// Examples: physical inventory count correction, accidental loss registration,
    ///           spillage recording, data entry error correction.
    ///
    /// Typical steps:
    ///   1. Adjustment  — quantity corrected with documented reason
    ///   2. Transfer    — custody adjustment if holder changes (optional)
    /// </summary>
    InventoryAdjustment,

    /// <summary>
    /// Generation and temporary storage of residues at the generation site
    /// without immediate transfer to an external manager.
    /// Common in industrial and agricultural contexts.
    /// Examples: industrial residue accumulation before scheduled pickup,
    ///           agricultural organic residue composting on-site.
    ///
    /// Typical steps:
    ///   1. Generation  — residues produced at site
    ///   2. Storage     — temporary accumulation at generation site
    ///   3. Transfer    — custody transferred when external manager arrives (optional)
    /// </summary>
    InSiteStorage,

    /// <summary>
    /// Combined physical processing followed by chemical transformation.
    /// Used when residues require preparation before treatment.
    /// Examples: plastic shredding before melting,
    ///           organic waste grinding before anaerobic digestion.
    ///
    /// Typical steps:
    ///   1. Relocation     — residue enters facility
    ///   2. Processing     — physical form prepared for transformation
    ///   3. Transformation — nature changes
    ///   4. Valorization   — output product enters marketplace (if applicable)
    ///   5. Relocation     — output routed to storage or next process
    /// </summary>
    IntegratedTreatment,

    /// <summary>
    /// Exit of residues from the system as products with commercial value.
    /// The residue transitions to Disposed and enters the residue marketplace.
    /// Applies to any residue type with recoverable value.
    /// Examples: recycled materials, recovered metals, reclaimed solvents.
    ///
    /// Typical steps:
    ///   1. Relocation   — residue prepared for valorization
    ///   2. Valorization — residue exits system as commercial product
    /// </summary>
    Valorization,

    /// <summary>
    /// Anaerobic digestion of organic residues producing biogas and digestate.
    /// A specialized IntegratedTreatment that produces two outputs:
    ///   - Biogas: exits system as energy product via Valorization
    ///   - Digestate: remains in system as a new residue
    /// Examples: food waste digestion, agricultural residue digestion,
    ///           sewage sludge digestion.
    ///
    /// Typical steps:
    ///   1. Relocation     — organic residue enters digestion facility
    ///   2. Splitting      — residue divided into biogas and digestate fractions
    ///   3. Transformation — anaerobic digestion occurs
    ///   4. Valorization   — biogas exits system as energy product
    ///   5. Relocation     — digestate routed to storage or further treatment
    /// </summary>
    BiogasProduction,

    /// <summary>
    /// Exit of residues from the system without commercial value,
    /// transferred directly to a recipient organization.
    /// Applies to any residue type in usable condition.
    /// The residue transitions to Disposed status.
    /// Examples: used clothing to NGOs, functional electronics to schools,
    ///           near-expiry food to food banks.
    ///
    /// Typical steps:
    ///   1. Relocation  — residue prepared for donation
    ///   2. Donation    — residue exits system to recipient
    ///   3. Transfer    — custody transferred to recipient organization
    /// </summary>
    Donation,

    /// <summary>
    /// Return of a rejected or refused residue back to its origin or
    /// to an alternative destination.
    /// Functionally identical to Transport with inverted origin and destination.
    /// Generates a new transport manifest — does not reactivate the original.
    /// Examples: residue rejected at destination due to manifest mismatch,
    ///           residue refused due to capacity constraints.
    ///
    /// Typical steps:
    ///   1. Transfer    — receiver returns custody to transporter
    ///   2. Relocation  — residue loaded back onto vehicle
    ///   3. Storage     — temporary storage on vehicle during return route (optional)
    ///   4. Relocation  — residue arrives at origin or alternative destination
    ///   5. Transfer    — custody returned to original generator or new holder
    /// </summary>
    Return
}
