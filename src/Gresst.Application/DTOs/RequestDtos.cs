namespace Gresst.Application.DTOs;

/// <summary>
/// API-specific request DTOs (not in Application/Infrastructure).
/// </summary>
public class ValidateTokenRequest
{
    public string Token { get; set; } = string.Empty;
}

public class LogoutRequest
{
    public string? RefreshToken { get; set; }
}

public class PublishToBankDto
{
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
}

public class ReceiveWasteDto
{
    public string WasteId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public string FacilityId { get; set; } = string.Empty;
}

public class SellWasteDto
{
    public string WasteId { get; set; } = string.Empty;
    public string BuyerId { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class DeliverWasteDto
{
    public string WasteId { get; set; } = string.Empty;
    public string RecipientId { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class ClassifyWasteDto
{
    public string WasteId { get; set; } = string.Empty;
    public string WasteClassId { get; set; } = string.Empty;
    public string ClassifiedById { get; set; } = string.Empty;
}
