public enum MovementType
{
    Collection,
    Relocation,     // Changes location, same owner
    Dispatch,       // Sends to service execution
    Return,         // Returns to generator
    Reception,      // Enters the installation
    Consolidation,  // Consolidation for transport
    Sampling        // Sampling (without moving the total)
}
