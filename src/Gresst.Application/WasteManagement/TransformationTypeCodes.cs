using Gresst.Domain.Enums;

namespace Gresst.Application.WasteManagement;

/// <summary>
/// Transformation type codes and mapping from waste_management_states.docx §6.
/// MERGE, SPLIT, CONVERT, BLEND, REPACK, CO_PROCESS, DERIVE.
/// </summary>
public static class TransformationTypeCodes
{
    public const string Merge = "MERGE";
    public const string Split = "SPLIT";
    public const string Convert = "CONVERT";
    public const string Blend = "BLEND";
    public const string Repack = "REPACK";
    public const string CoProcess = "CO_PROCESS";
    public const string Derive = "DERIVE";

    public static TransformationType ToTransformationType(string code)
    {
        return code?.ToUpperInvariant() switch
        {
            Merge => TransformationType.Grouping,
            Split => TransformationType.Decomposition,
            Convert => TransformationType.Conversion,
            Blend => TransformationType.Blend,
            Repack => TransformationType.Repack,
            CoProcess => TransformationType.Coprocess,
            Derive => TransformationType.Derive,
            _ => TransformationType.Conversion
        };
    }

    public static string ToCode(TransformationType type)
    {
        return type switch
        {
            TransformationType.Grouping => Merge,
            TransformationType.Decomposition => Split,
            TransformationType.Conversion => Convert,
            TransformationType.Blend => Blend,
            TransformationType.Repack => Repack,
            TransformationType.Coprocess => CoProcess,
            TransformationType.Derive => Derive,
            _ => Convert
        };
    }
}
