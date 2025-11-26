namespace Gresst.Application.DTOs;

public class UpdateRequestDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string[]? ServicesRequested { get; set; }
    public DateTime? RequiredByDate { get; set; }
    public string? PickupAddress { get; set; }
    public string? DeliveryAddress { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? AgreedCost { get; set; }
    public Guid? ProviderId { get; set; }
}

