using Gresst.Domain.RouteProcesses;

namespace Gresst.Application.RouteProcesses;

public static class RouteProcessMapping
{
    public static RouteProcessDto ToDto(RouteProcess route)
    {
        var stops = route.Stops
            .Select(s => new RouteStopDto
            {
                Id = s.Id,
                LocationId = s.LocationId,
                Order = s.Order,
                OperationType = s.OperationType,
                ResponsiblePartyId = s.ResponsiblePartyId,
                IsCompleted = s.IsCompleted,
                CompletedAt = s.CompletedAt,
                Notes = s.Notes
            })
            .ToList();

        var completed = route.Stops.Count(s => s.IsCompleted);
        var total = route.Stops.Count;
        var progressPercent = total > 0 ? (int)Math.Round(100.0 * completed / total) : 0;

        return new RouteProcessDto
        {
            Id = route.Id,
            VehicleId = route.VehicleId,
            DriverId = route.DriverId,
            Status = route.Status,
            CreatedAt = route.CreatedAt,
            StartedAt = route.StartedAt,
            CompletedAt = route.CompletedAt,
            CancelledAt = route.CancelledAt,
            CancellationReason = route.CancellationReason,
            Stops = stops,
            ProgressPercent = progressPercent
        };
    }
}
