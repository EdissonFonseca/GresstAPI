using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

/// <summary>
/// Controller for managing processes, subprocesses, and tasks
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ProcessController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestService? _requestService;

    public ProcessController(
        ICurrentUserService currentUserService,
        IRequestService? requestService = null)
    {
        _currentUserService = currentUserService;
        _requestService = requestService;
    }

    /// <summary>
    /// Get transport requests as processes with subprocesses and tasks
    /// </summary>
    /// <remarks>
    /// Returns transport requests structured as:
    /// - Process: Each request (grouped by IdGrupo or IdSolicitud)
    /// - SubProcess: Each order (IdOrden) within a request
    /// - Task: Each transport item within an order or request
    /// </remarks>
    [HttpGet("transport-requests")]
    [ProducesResponseType(typeof(IEnumerable<ProcessDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<IEnumerable<ProcessDto>>> GetTransportRequests(
        CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        var personId = _currentUserService.GetCurrentPersonId();
        
        if (personId == Guid.Empty)
        {
            return BadRequest(new { message = "Person ID not found for current user" });
        }

        var transportData = await _requestService.GetMobileTransportWasteAsync(personId, cancellationToken);
        var processes = MapTransportDataToProcesses(transportData);
        
        return Ok(processes);
    }

    /// <summary>
    /// Maps transport waste data to process hierarchy
    /// </summary>
    private List<ProcessDto> MapTransportDataToProcesses(IEnumerable<MobileTransportWasteDto> transportData)
    {
        var processes = new List<ProcessDto>();
        
        // Group by IdGrupo (which combines IdSolicitud and IdDepositoOrigen)
        var groupedByProcess = transportData
            .GroupBy(d => d.IdGrupo ?? $"Request-{d.IdSolicitud}")
            .ToList();

        foreach (var processGroup in groupedByProcess)
        {
            var firstItem = processGroup.First();
            
            // Create Process from request
            var process = new ProcessDto
            {
                Id = Guid.NewGuid(), // Generate or use IdSolicitud if available
                Name = firstItem.Titulo ?? $"Transport Request {firstItem.NumeroSolicitud ?? firstItem.IdSolicitud}",
                Description = $"Transport process for request {firstItem.NumeroSolicitud ?? firstItem.IdSolicitud}",
                Status = MapStatus(firstItem.IdEstado, firstItem.IdEtapa, firstItem.IdFase),
                StartDate = firstItem.FechaInicio,
                DueDate = firstItem.FechaSolicitud,
                Priority = DeterminePriority(firstItem),
                Metadata = new Dictionary<string, object>
                {
                    { "IdSolicitud", firstItem.IdSolicitud },
                    { "NumeroSolicitud", firstItem.NumeroSolicitud ?? 0 },
                    { "IdGrupo", firstItem.IdGrupo ?? "" },
                    { "Solicitante", firstItem.Solicitante ?? "" },
                    { "Proveedor", firstItem.Proveedor ?? "" }
                }
            };

            // Group items by Order (IdOrden) to create SubProcesses
            var orderGroups = processGroup
                .Where(d => d.IdOrden.HasValue)
                .GroupBy(d => d.IdOrden!.Value)
                .ToList();

            // Create SubProcesses for each order
            foreach (var orderGroup in orderGroups)
            {
                var orderFirstItem = orderGroup.First();
                var subProcess = new SubProcessDto
                {
                    Id = Guid.NewGuid(),
                    ProcessId = process.Id,
                    Name = $"Order {orderFirstItem.NumeroOrden ?? orderFirstItem.IdOrden}",
                    Description = $"Transport order {orderFirstItem.NumeroOrden ?? orderFirstItem.IdOrden}",
                    Status = MapStatus(orderFirstItem.IdEstado, orderFirstItem.IdEtapa, orderFirstItem.IdFase),
                    StartDate = orderFirstItem.FechaInicio,
                    Priority = DeterminePriority(orderFirstItem),
                    Metadata = new Dictionary<string, object>
                    {
                        { "IdOrden", orderFirstItem.IdOrden!.Value },
                        { "NumeroOrden", orderFirstItem.NumeroOrden ?? 0 },
                        { "IdVehiculo", orderFirstItem.IdVehiculo ?? "" },
                        { "IdResponsable", orderFirstItem.IdResponsable ?? "" }
                    }
                };

                // Create Tasks for each item in the order
                foreach (var item in orderGroup)
                {
                    var task = CreateTaskFromTransportItem(item, process.Id, subProcess.Id);
                    subProcess.Tasks.Add(task);
                }

                process.SubProcesses.Add(subProcess);
            }

            // Create Tasks for items without orders (directly under process)
            var itemsWithoutOrder = processGroup
                .Where(d => !d.IdOrden.HasValue)
                .ToList();

            foreach (var item in itemsWithoutOrder)
            {
                var task = CreateTaskFromTransportItem(item, process.Id, null);
                process.Tasks.Add(task);
            }

            processes.Add(process);
        }

        return processes;
    }

    /// <summary>
    /// Creates a TaskDto from a transport waste item
    /// </summary>
    private TaskDto CreateTaskFromTransportItem(
        MobileTransportWasteDto item, 
        Guid processId, 
        Guid? subProcessId)
    {
        return new TaskDto
        {
            Id = Guid.NewGuid(),
            ProcessId = processId,
            SubProcessId = subProcessId,
            Name = $"{item.Material ?? "Material"} - {item.DepositoOrigen ?? "Origin"} to {item.DepositoDestino ?? "Destination"}",
            Description = item.Descripcion ?? $"Transport task for {item.Material}",
            Status = MapStatus(item.IdEstado, item.IdEtapa, item.IdFase),
            StartDate = item.FechaInicio,
            AssignedTo = item.IdResponsable ?? item.IdResponsable2,
            Priority = DeterminePriority(item),
            Metadata = new Dictionary<string, object>
            {
                { "Item", item.Item },
                { "IdMaterial", item.IdMaterial ?? 0 },
                { "Material", item.Material ?? "" },
                { "Cantidad", item.Cantidad ?? 0 },
                { "Peso", item.Peso ?? 0 },
                { "Volumen", item.Volumen ?? 0 },
                { "IdDepositoOrigen", item.IdDepositoOrigen ?? 0 },
                { "DepositoOrigen", item.DepositoOrigen ?? "" },
                { "DireccionOrigen", item.DireccionOrigen ?? "" },
                { "LatitudOrigen", item.LatitudOrigen ?? 0 },
                { "LongitudOrigen", item.LongitudOrigen ?? 0 },
                { "IdDepositoDestino", item.IdDepositoDestino ?? 0 },
                { "DepositoDestino", item.DepositoDestino ?? "" },
                { "DireccionDestino", item.DireccionDestino ?? "" },
                { "LatitudDestino", item.LatitudDestino ?? 0 },
                { "LongitudDestino", item.LongitudDestino ?? 0 },
                { "Tratamiento", item.Tratamiento ?? "" },
                { "Embalaje", item.Embalaje ?? "" },
                { "PrecioUnitario", item.PrecioUnitario ?? 0 },
                { "PrecioServicioUnitario", item.PrecioServicioUnitario ?? 0 }
            }
        };
    }

    /// <summary>
    /// Maps database status codes to process status
    /// </summary>
    private string MapStatus(string? idEstado, string? idEtapa, string? idFase)
    {
        if (string.IsNullOrEmpty(idEstado))
            return "Unknown";

        // Map common status codes
        return idEstado switch
        {
            "P" => "Pending",
            "A" => "Approved",
            "R" => "Rejected",
            "C" => "Cancelled",
            "E" => "InProgress",
            "F" => "Completed",
            _ => idEstado
        };
    }

    /// <summary>
    /// Determines priority based on dates and status
    /// </summary>
    private string DeterminePriority(MobileTransportWasteDto item)
    {
        if (item.FechaInicio.HasValue && item.FechaInicio.Value < DateTime.Now.AddDays(1))
            return "High";
        
        if (item.FechaInicio.HasValue && item.FechaInicio.Value < DateTime.Now.AddDays(3))
            return "Medium";
        
        return "Low";
    }
}

