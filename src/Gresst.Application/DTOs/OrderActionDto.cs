namespace Gresst.Application.DTOs;

/// <summary>
/// DTO for scheduling an order
/// </summary>
public class ScheduleOrderDto
{
    public DateTime ScheduledDate { get; set; }
    public string? VehicleId { get; set; }
    public string? RouteId { get; set; }
}

/// <summary>
/// DTO for completing an order
/// </summary>
public class CompleteOrderDto
{
    public decimal? ActualCost { get; set; }
}

/// <summary>
/// DTO for cancelling an order
/// </summary>
public class CancelOrderDto
{
    public string? Reason { get; set; }
}

