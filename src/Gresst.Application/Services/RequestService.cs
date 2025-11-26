using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing Requests (Solicitudes)
/// </summary>
public class RequestService : IRequestService
{
    private readonly IRepository<Request> _requestRepository;
    private readonly IRequestRepository _requestRepositoryInfra;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RequestService(
        IRepository<Request> requestRepository,
        IRequestRepository requestRepositoryInfra,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _requestRepository = requestRepository;
        _requestRepositoryInfra = requestRepositoryInfra;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<RequestDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(id, cancellationToken);
        if (request == null)
            return null!;

        return MapToDto(request);
    }

    public async Task<IEnumerable<RequestDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var requests = await _requestRepository.GetAllAsync(cancellationToken);
        return requests.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<RequestDto>> GetByRequesterAsync(Guid requesterId, CancellationToken cancellationToken = default)
    {
        var requests = await _requestRepository.FindAsync(
            r => r.RequesterId == requesterId,
            cancellationToken);
        return requests.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<RequestDto>> GetByProviderAsync(Guid providerId, CancellationToken cancellationToken = default)
    {
        var requests = await _requestRepository.FindAsync(
            r => r.ProviderId == providerId,
            cancellationToken);
        return requests.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<RequestDto>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<RequestStatus>(status, out var requestStatus))
        {
            return new List<RequestDto>();
        }

        var requests = await _requestRepository.FindAsync(
            r => r.Status == requestStatus,
            cancellationToken);
        return requests.Select(MapToDto).ToList();
    }

    public async Task<RequestDto> CreateAsync(CreateRequestDto dto, CancellationToken cancellationToken = default)
    {
        // Note: ServiceId is required but not in CreateRequestDto
        // For now, we'll need to handle this - might need to add ServiceId to CreateRequestDto
        // or use a default service. This is a placeholder implementation.
        var request = new Request
        {
            RequestNumber = GenerateRequestNumber(),
            Status = RequestStatus.Submitted,
            RequesterId = dto.RequesterId,
            ProviderId = dto.ProviderId,
            ServiceId = Guid.Empty, // TODO: Add ServiceId to CreateRequestDto or get from context
            Title = dto.Title,
            Description = dto.Description,
            ServicesRequested = string.Join(",", dto.ServicesRequested),
            RequestedDate = DateTime.UtcNow,
            RequiredByDate = dto.RequiredByDate,
            PickupAddress = dto.PickupAddress,
            DeliveryAddress = dto.DeliveryAddress,
            EstimatedCost = null,
            AgreedCost = null
        };

        // Add items
        foreach (var itemDto in dto.Items)
        {
            request.Items.Add(new RequestItem
            {
                WasteClassId = itemDto.WasteClassId,
                EstimatedQuantity = itemDto.EstimatedQuantity,
                Unit = itemDto.Unit,
                Description = itemDto.Description
            });
        }

        await _requestRepository.AddAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(request);
    }

    public async Task<RequestDto> UpdateAsync(dynamic dto, CancellationToken cancellationToken = default)
    {
        // This is a simplified implementation
        // In a real scenario, you'd want a proper UpdateRequestDto
        var updateDto = dto as UpdateRequestDto;
        if (updateDto == null)
            throw new ArgumentException("Invalid update DTO", nameof(dto));

        var request = await _requestRepository.GetByIdAsync(updateDto.Id, cancellationToken);
        if (request == null)
            return null!;

        // Update only provided fields
        if (!string.IsNullOrEmpty(updateDto.Title))
            request.Title = updateDto.Title;
        
        if (!string.IsNullOrEmpty(updateDto.Description))
            request.Description = updateDto.Description;
        
        if (updateDto.ServicesRequested != null)
            request.ServicesRequested = string.Join(",", updateDto.ServicesRequested);
        
        if (updateDto.RequiredByDate.HasValue)
            request.RequiredByDate = updateDto.RequiredByDate;
        
        if (!string.IsNullOrEmpty(updateDto.PickupAddress))
            request.PickupAddress = updateDto.PickupAddress;
        
        if (!string.IsNullOrEmpty(updateDto.DeliveryAddress))
            request.DeliveryAddress = updateDto.DeliveryAddress;
        
        if (updateDto.EstimatedCost.HasValue)
            request.EstimatedCost = updateDto.EstimatedCost;
        
        if (updateDto.AgreedCost.HasValue)
            request.AgreedCost = updateDto.AgreedCost;
        
        if (updateDto.ProviderId.HasValue)
            request.ProviderId = updateDto.ProviderId;

        await _requestRepository.UpdateAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(request);
    }

    public async Task<RequestDto> ApproveAsync(Guid id, decimal? agreedCost, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(id, cancellationToken);
        if (request == null)
            return null!;

        if (request.Status != RequestStatus.Submitted && request.Status != RequestStatus.UnderReview)
            throw new InvalidOperationException("Only submitted or under review requests can be approved");

        request.Status = RequestStatus.Approved;
        request.ApprovedDate = DateTime.UtcNow;
        if (agreedCost.HasValue)
            request.AgreedCost = agreedCost;

        await _requestRepository.UpdateAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(request);
    }

    public async Task<RequestDto> RejectAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(id, cancellationToken);
        if (request == null)
            return null!;

        if (request.Status != RequestStatus.Submitted && request.Status != RequestStatus.UnderReview)
            throw new InvalidOperationException("Only submitted or under review requests can be rejected");

        request.Status = RequestStatus.Rejected;
        request.Description = $"{request.Description}\n\nRejection reason: {reason}";

        await _requestRepository.UpdateAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(request);
    }

    public async Task CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(id, cancellationToken);
        if (request == null)
            return;

        request.Status = RequestStatus.Cancelled;

        await _requestRepository.UpdateAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<MobileTransportWasteDto>> GetMobileTransportWasteAsync(
        Guid personId, 
        CancellationToken cancellationToken = default)
    {
        return await _requestRepositoryInfra.GetMobileTransportWasteAsync(personId, cancellationToken);
    }

    // Helper methods
    private RequestDto MapToDto(Request request)
    {
        return new RequestDto
        {
            Id = request.Id,
            RequestNumber = request.RequestNumber,
            Status = request.Status.ToString(),
            RequesterId = request.RequesterId,
            RequesterName = request.Requester?.Name ?? string.Empty,
            ProviderId = request.ProviderId,
            ProviderName = request.Provider?.Name,
            Title = request.Title,
            Description = request.Description,
            ServicesRequested = request.ServicesRequested.Split(',', StringSplitOptions.RemoveEmptyEntries),
            RequestedDate = request.RequestedDate,
            RequiredByDate = request.RequiredByDate,
            EstimatedCost = request.EstimatedCost,
            AgreedCost = request.AgreedCost,
            Items = request.Items.Select(item => new RequestItemDto
            {
                Id = item.Id,
                WasteClassId = item.WasteClassId,
                WasteClassName = item.WasteClass?.Name ?? string.Empty,
                EstimatedQuantity = item.EstimatedQuantity,
                Unit = item.Unit.ToString(),
                Description = item.Description
            }).ToList()
        };
    }

    private string GenerateRequestNumber()
    {
        // Simple implementation - in production, use a proper numbering service
        return $"REQ-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
    }
}

