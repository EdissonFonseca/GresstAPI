public class DisposalProcess : AggregateRoot
{
    public Guid Id { get; }
    public Guid FacilityId { get; }
    public string PermitNumber { get; }

    public DisposalProcess(Guid id, Guid facilityId, string permitNumber)
    {
        Id = id;
        FacilityId = facilityId;
        PermitNumber = permitNumber;
    }
}