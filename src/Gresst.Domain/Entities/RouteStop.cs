public class RouteStop
{
    public string Id { get; init; }
    public string LocationId { get; }
    public int Order { get; }

    public RouteStop(string id, string locationId, int order)
    {
        Id = id;
        LocationId = locationId;
        Order = order;
    }
}