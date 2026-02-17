namespace Gresst.Domain.Enums;

public enum TransformationType
{
    Conversion = 1,      // Convert one waste type to another
    Decomposition = 2,   // Break down (split) into multiple wastes
    Grouping = 3,        // Combine (merge) multiple wastes into one
    Treatment = 4,       // Apply treatment process
    Blend = 5,
    Repack = 6,
    Coprocess = 7,
    Derive = 8
}

