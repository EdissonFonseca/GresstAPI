using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing processes, subprocesses, and tasks
/// </summary>
public class ProcessService : IProcessService
{
    /// <summary>
    /// Maps transport waste data to process hierarchy:
    /// Process  = Order (IdOrden / vehicle)
    /// SubProcess = Collection point (OrdenPlaneacion → request + origin depot)
    /// Task    = Request detail (SolicitudDetalle) at that point
    /// </summary>
    public Task<IEnumerable<ProcessDto>> MapTransportDataToProcessesAsync(
        IEnumerable<MobileTransportWasteDto> transportData,
        CancellationToken cancellationToken = default)
    {
        var processes = MapTransportDataToProcesses(transportData);
        return Task.FromResult<IEnumerable<ProcessDto>>(processes);
    }

    /// <summary>
    /// Maps transport waste data to process hierarchy:
    /// Process  = Order (IdOrden / vehicle)
    /// SubProcess = Collection point (OrdenPlaneacion → request + origin depot)
    /// Task    = Request detail (SolicitudDetalle) at that point
    /// </summary>
    private List<ProcessDto> MapTransportDataToProcesses(IEnumerable<MobileTransportWasteDto> transportData)
    {
        var processes = new List<ProcessDto>();
        var data = transportData.ToList();

        // 1) Processes: group by Order (IdOrden) – one process per service order / vehicle
        var orderGroups = data
            .Where(d => d.IdOrden.HasValue)
            .GroupBy(d => d.IdOrden!.Value)
            .ToList();

        foreach (var orderGroup in orderGroups)
        {
            var firstOrderItem = orderGroup.First();

            // Process = Service Order associated to a vehicle
            var process = new ProcessDto
            {
                Id = Guid.NewGuid(),
                Name = $"Order {firstOrderItem.NumeroOrden ?? firstOrderItem.IdOrden} - Vehicle {firstOrderItem.IdVehiculo ?? "N/A"}",
                Description = $"Transport order {firstOrderItem.NumeroOrden ?? firstOrderItem.IdOrden} for vehicle {firstOrderItem.IdVehiculo ?? "N/A"}",
                Status = MapStatus(firstOrderItem.IdEstado, firstOrderItem.IdEtapa, firstOrderItem.IdFase),
                StartDate = orderGroup.Min(i => i.FechaInicio) ?? firstOrderItem.FechaInicio,
                DueDate = orderGroup.Min(i => i.FechaSolicitud) ?? firstOrderItem.FechaSolicitud,
                Priority = DeterminePriority(firstOrderItem),
                Metadata = new Dictionary<string, object>
                {
                    { "IdOrden", firstOrderItem.IdOrden!.Value },
                    { "NumeroOrden", firstOrderItem.NumeroOrden ?? 0 },
                    { "IdVehiculo", firstOrderItem.IdVehiculo ?? "" },
                    { "Solicitudes", orderGroup.Select(i => i.IdSolicitud).Distinct().ToList() }
                }
            };

            // 2) SubProcesses: group by collection point (IdGrupo = IdSolicitud-IdDepositoOrigen)
            var collectionPointGroups = orderGroup
                .GroupBy(i => i.IdGrupo ?? $"Req-{i.IdSolicitud}-Origin-{i.IdDepositoOrigen ?? 0}")
                .ToList();

            foreach (var cpGroup in collectionPointGroups)
            {
                var cpFirst = cpGroup.First();

                var subProcess = new SubProcessDto
                {
                    Id = Guid.NewGuid(),
                    ProcessId = process.Id,
                    Name = $"Pickup at {cpFirst.DepositoOrigen ?? "Unknown origin"}",
                    Description = $"Collection point at {cpFirst.DepositoOrigen ?? "origin depot"} for order {firstOrderItem.NumeroOrden ?? firstOrderItem.IdOrden}",
                    Status = MapStatus(cpFirst.IdEstado, cpFirst.IdEtapa, cpFirst.IdFase),
                    StartDate = cpGroup.Min(i => i.FechaInicio) ?? cpFirst.FechaInicio,
                    Priority = DeterminePriority(cpFirst),
                    Metadata = new Dictionary<string, object>
                    {
                        { "IdGrupo", cpFirst.IdGrupo ?? "" },
                        { "IdSolicitud", cpFirst.IdSolicitud },
                        { "IdDepositoOrigen", cpFirst.IdDepositoOrigen ?? 0 },
                        { "DepositoOrigen", cpFirst.DepositoOrigen ?? "" },
                        { "DireccionOrigen", cpFirst.DireccionOrigen ?? "" },
                        { "LatitudOrigen", cpFirst.LatitudOrigen ?? 0 },
                        { "LongitudOrigen", cpFirst.LongitudOrigen ?? 0 }
                    }
                };

                // 3) Tasks: each SolicitudDetalle (row) at this collection point
                foreach (var item in cpGroup)
                {
                    var task = CreateTaskFromTransportItem(item, process.Id, subProcess.Id);
                    subProcess.Tasks.Add(task);
                }

                process.SubProcesses.Add(subProcess);
            }

            processes.Add(process);
        }

        // Optional: handle items without an associated order as standalone processes
        var withoutOrder = data.Where(d => !d.IdOrden.HasValue).ToList();
        if (withoutOrder.Any())
        {
            var fallbackGroups = withoutOrder
                .GroupBy(i => i.IdGrupo ?? $"Req-{i.IdSolicitud}-Origin-{i.IdDepositoOrigen ?? 0}")
                .ToList();

            foreach (var group in fallbackGroups)
            {
                var first = group.First();
                var process = new ProcessDto
                {
                    Id = Guid.NewGuid(),
                    Name = first.Titulo ?? $"Unscheduled transport for request {first.NumeroSolicitud ?? first.IdSolicitud}",
                    Description = "Unscheduled transport without service order",
                    Status = MapStatus(first.IdEstado, first.IdEtapa, first.IdFase),
                    StartDate = group.Min(i => i.FechaInicio) ?? first.FechaInicio,
                    DueDate = group.Min(i => i.FechaSolicitud) ?? first.FechaSolicitud,
                    Priority = DeterminePriority(first),
                    Metadata = new Dictionary<string, object>
                    {
                        { "IdSolicitud", first.IdSolicitud },
                        { "NumeroSolicitud", first.NumeroSolicitud ?? 0 },
                        { "IdGrupo", first.IdGrupo ?? "" }
                    }
                };

                var subProcess = new SubProcessDto
                {
                    Id = Guid.NewGuid(),
                    ProcessId = process.Id,
                    Name = $"Pickup at {first.DepositoOrigen ?? "Unknown origin"}",
                    Description = "Collection point without associated order",
                    Status = MapStatus(first.IdEstado, first.IdEtapa, first.IdFase),
                    StartDate = group.Min(i => i.FechaInicio) ?? first.FechaInicio,
                    Priority = DeterminePriority(first),
                    Metadata = new Dictionary<string, object>
                    {
                        { "IdGrupo", first.IdGrupo ?? "" },
                        { "IdDepositoOrigen", first.IdDepositoOrigen ?? 0 },
                        { "DepositoOrigen", first.DepositoOrigen ?? "" }
                    }
                };

                foreach (var item in group)
                {
                    var task = CreateTaskFromTransportItem(item, process.Id, subProcess.Id);
                    subProcess.Tasks.Add(task);
                }

                process.SubProcesses.Add(subProcess);
                processes.Add(process);
            }
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

