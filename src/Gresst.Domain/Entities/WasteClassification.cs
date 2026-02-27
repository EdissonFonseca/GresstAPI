public class WasteClassification {
    //Based on LER code
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ParentId { get; set; }
    public bool IsHazardous { get; set; }
    public bool HasMirrorEntry { get; set; }
    public string MirrorEntryId { get; set; }
}