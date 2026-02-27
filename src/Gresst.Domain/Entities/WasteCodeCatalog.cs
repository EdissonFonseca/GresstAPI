using Gresst.Domain.Common;

/// Master catalog of a coding system — reference only
public class WasteCodeCatalog : BaseEntity
{
    public string System { get; set; }      // "LER", "UN", "Basel-Y", "Basel-A"
    public string? Description { get; set; }

    public WasteCodeCatalog(string system)
    {
        System = system;
    }
}