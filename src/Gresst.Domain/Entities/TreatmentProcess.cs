public class TreatmentProcess : AggregateRoot
{
    public Guid Id { get; }
    public Guid FacilityId { get; }
    public ProcessStatus Status { get; private set; }
}