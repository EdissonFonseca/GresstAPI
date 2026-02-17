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
    private readonly IRequestRepository _solicitudRepository;
    private readonly IProcessService _processService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RequestService(
        IRepository<Request> requestRepository,
        IRequestRepository solicitudRepository,
        IProcessService processService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _requestRepository = requestRepository;
        _solicitudRepository = solicitudRepository;
        _processService = processService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<RequestDto> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!long.TryParse(id, out var idSolicitud))
            return null!;

        var filter = BuildSolicitudFilterForCurrentUser();
        filter.SolicitudIds = new[] { idSolicitud };
        var list = (await _solicitudRepository.GetAllAsync(filter, cancellationToken)).ToList();
        var request = list.FirstOrDefault(r => r.Id == id);
        return request == null ? null! : MapRequestToDto(request);
    }

    public async Task<IEnumerable<RequestDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var filter = BuildSolicitudFilterForCurrentUser();
        var list = await _solicitudRepository.GetAllAsync(filter, cancellationToken);
        return list.Select(MapRequestToDto).ToList();
    }

    public async Task<IEnumerable<RequestDto>> GetByRequesterAsync(string requesterId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(requesterId))
            return new List<RequestDto>();
        var filter = BuildSolicitudFilterForCurrentUser();
        filter.SolicitanteIds = new[] { requesterId };
        var list = await _solicitudRepository.GetAllAsync(filter, cancellationToken);
        return list.Select(MapRequestToDto).ToList();
    }

    public async Task<IEnumerable<RequestDto>> GetByProviderAsync(string providerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(providerId))
            return new List<RequestDto>();
        var filter = BuildSolicitudFilterForCurrentUser();
        filter.ProveedorIds = new[] { providerId };
        var list = await _solicitudRepository.GetAllAsync(filter, cancellationToken);
        return list.Select(MapRequestToDto).ToList();
    }

    public async Task<IEnumerable<RequestDto>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(status))
            return new List<RequestDto>();
        var filter = BuildSolicitudFilterForCurrentUser();
        filter.Estados = new[] { status };
        var list = await _solicitudRepository.GetAllAsync(filter, cancellationToken);
        return list.Select(MapRequestToDto).ToList();
    }

    /// <summary>
    /// Builds a filter with AccountPersonId set to the current user (multitenant: only rows where Solicitud.IdPersona == accountPersonId).
    /// </summary>
    private SolicitudFilter BuildSolicitudFilterForCurrentUser()
    {
        var filter = new SolicitudFilter();
        var accountPersonId = _currentUserService.GetCurrentAccountPersonId();
        if (!string.IsNullOrEmpty(accountPersonId))
            filter.AccountPersonId = accountPersonId;
        return filter;
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
            ServiceId = string.Empty, // TODO: Add ServiceId to CreateRequestDto or get from context
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
        
        if (!string.IsNullOrEmpty(updateDto.ProviderId))
            request.ProviderId = updateDto.ProviderId;

        await _requestRepository.UpdateAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(request);
    }

    public async Task<RequestDto> ApproveAsync(string id, decimal? agreedCost, CancellationToken cancellationToken = default)
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

    public async Task<RequestDto> RejectAsync(string id, string reason, CancellationToken cancellationToken = default)
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

    public async Task CancelAsync(string id, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(id, cancellationToken);
        if (request == null)
            return;

        request.Status = RequestStatus.Cancelled;

        await _requestRepository.UpdateAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<MobileTransportWasteDto>> GetMobileTransportWasteAsync(
        string personId,
        CancellationToken cancellationToken = default)
    {
        return await _processService.GetMobileTransportWasteForAccountAsync(personId, cancellationToken);
    }

    // Map domain Request to API RequestDto (repository returns domain, service maps to DTO)
    private static RequestDto MapRequestToDto(Gresst.Domain.Entities.Request request)
    {
        return new RequestDto
        {
            Id = request.Id,
            RequestNumber = request.RequestNumber,
            Status = request.Status.ToString(),
            RequesterId = request.RequesterId,
            RequesterName = request.Requester?.Name ?? "",
            ProviderId = request.ProviderId,
            ProviderName = request.Provider?.Name,
            Title = request.Title,
            Description = request.Description,
            ServicesRequested = request.ServicesRequested?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(),
            RequestedDate = request.RequestedDate,
            RequiredByDate = request.RequiredByDate,
            EstimatedCost = request.EstimatedCost,
            AgreedCost = request.AgreedCost,
            Items = request.Items?.Select(item => new RequestItemDto
            {
                Id = item.Id,
                WasteClassId = item.WasteClassId,
                WasteClassName = item.WasteClass?.Name ?? "",
                EstimatedQuantity = item.EstimatedQuantity,
                Unit = item.Unit.ToString(),
                Description = item.Description
            }).ToList() ?? new List<RequestItemDto>()
        };
    }

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

