public enum OperationType
{
    Generation = 1,         // Generation of residues. Dimension: Existence
    Relocation,         // Movement of residues. Dimension: Location
    Transfer,           // Transference of property of residues. Dimension: Ownership
    Processing,         // Processing. Dimension: Physical Form
    Transformation,     // Transformation. Dimension: Nature
    Disposal,           // Disposal. Dimension: Existence
    Storage,            // Temporary storage. Dimension: Operational state
    Containment,        // Permanent storage. Dimension: Operational state    
    Adjustment          // Adjustment of inventory. Dimension: Quantity
}