namespace Gresst.Application.DTOs;

/// <summary>
/// DTO for approving a request
/// </summary>
public class ApproveRequestDto
{
    public decimal? AgreedCost { get; set; }
}

/// <summary>
/// DTO for rejecting a request
/// </summary>
public class RejectRequestDto
{
    public string Reason { get; set; } = string.Empty;
}

