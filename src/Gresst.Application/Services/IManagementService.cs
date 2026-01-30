using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service for waste management operations (all 11 operations)
/// </summary>
public interface IManagementService
{
    // Generic management operation
    Task<ManagementDto> CreateManagementAsync(CreateManagementDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<ManagementDto>> GetWasteHistoryAsync(string wasteId, CancellationToken cancellationToken = default);
    
    // Specific operations (implements the 11 required operations)
    Task<ManagementDto> GenerateWasteAsync(CreateWasteDto wasteDto, CancellationToken cancellationToken = default);
    Task<ManagementDto> CollectWasteAsync(CollectWasteDto dto, CancellationToken cancellationToken = default);
    Task<ManagementDto> TransportWasteAsync(TransportWasteDto dto, CancellationToken cancellationToken = default);
    Task<ManagementDto> ReceiveWasteAsync(string wasteId, string receiverId, string facilityId, CancellationToken cancellationToken = default);
    Task<ManagementDto> StoreWasteAsync(StoreWasteDto dto, CancellationToken cancellationToken = default);
    Task<ManagementDto> DisposeWasteAsync(DisposeWasteDto dto, CancellationToken cancellationToken = default);
    Task<ManagementDto> TransformWasteAsync(TransformWasteDto dto, CancellationToken cancellationToken = default);
    Task<ManagementDto> ClassifyWasteAsync(string wasteId, string wasteTypeId, string classifiedById, CancellationToken cancellationToken = default);
    Task<ManagementDto> SellWasteAsync(string wasteId, string buyerId, decimal price, CancellationToken cancellationToken = default);
    Task<ManagementDto> DeliverToThirdPartyAsync(string wasteId, string recipientId, string notes, CancellationToken cancellationToken = default);
}

