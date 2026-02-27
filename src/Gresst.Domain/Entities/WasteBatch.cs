/// <summary>
/// A Batch of waste items that are grouped together for processing, transport, or storage. It allows for easier handling and tracking of multiple waste items that share similar characteristics or are generated at the same time.
/// </summary>
public class WasteBatch {
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string GeneratorId { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = string.Empty;
}