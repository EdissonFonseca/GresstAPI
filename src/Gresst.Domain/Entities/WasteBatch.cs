public class WasteBatch {
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string generatorId { get; set; } = string.Empty;
    public DateTime receivedAt { get; set; } = DateTime.UtcNow;
    public string status { get; set; } = string.Empty;
}