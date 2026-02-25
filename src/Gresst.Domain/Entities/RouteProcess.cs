public class RouteProcess : AggregateRoot
{
    public Guid Id { get; }
    public Guid VehicleId { get; }
    public RouteStatus Status { get; private set; }

    private readonly List<RouteStop> _stops = new();
}