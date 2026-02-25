public class IncidentProcess : AggregateRoot
{
    public Guid Id { get; }
    public IncidentType Type { get; }
    public IncidentSeverity Severity { get; }
    public bool ReportedToAuthority { get; private set; }
}