using Gresst.Application.DTOs;
using Gresst.Application.WasteManagement;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing processes, subprocesses, and tasks
/// </summary>
public class ProcessService : IProcessService
{
    private readonly IRequestRepository _requestRepository;
    private readonly IRequestFilterDefaults _requestFilterDefaults;

    public ProcessService(IRequestRepository requestRepository, IRequestFilterDefaults requestFilterDefaults)
    {
        _requestRepository = requestRepository;
        _requestFilterDefaults = requestFilterDefaults;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MobileTransportWasteDto>> GetMobileTransportWasteForAccountAsync(
        string accountPersonId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(accountPersonId))
            return Array.Empty<MobileTransportWasteDto>();

        var filter = new SolicitudFilter
        {
            PersonIds = new[] { accountPersonId },
            IdServicio = _requestFilterDefaults.GetTransportServiceId(),
            Estados = _requestFilterDefaults.GetActiveEstadosForTransport(),
            ExcludeRecurring = true
        };

        var solicitudes = await _requestRepository.GetSolicitudesAsync(filter, cancellationToken);
        var list = solicitudes.ToList();

        var filtered = list.Where(WasteManagementRules.IsIncludedInMobileTransport).ToList();

        var keys = filtered
            .Where(s => s.IdDepositoOrigen.HasValue && s.IdDepositoOrigen.Value != 0)
            .Select(s => (s.IdSolicitud, s.IdDepositoOrigen!.Value))
            .Distinct()
            .ToList();

        var planning = keys.Count > 0
            ? await _requestRepository.GetOrdenPlanningForSolicitudesAsync(keys, cancellationToken)
            : new List<OrdenPlanningDto>();
        var planningDict = planning.ToDictionary(p => (p.IdSolicitud, p.IdDepositoOrigen));

        return filtered.Select(s =>
        {
            var key = (IdSolicitud: s.IdSolicitud, IdDepositoOrigen: s.IdDepositoOrigen ?? 0L);
            var plan = key.IdDepositoOrigen != 0 && planningDict.TryGetValue(key, out var p) ? p : null;
            return new MobileTransportWasteDto
            {
                IdSolicitud = s.IdSolicitud,
                NumeroSolicitud = s.NumeroSolicitud,
                FechaSolicitud = s.FechaSolicitud,
                Ocurrencia = s.Ocurrencia,
                Recurrencia = s.Recurrencia,
                IdOrden = plan?.IdOrden,
                NumeroOrden = plan?.NumeroOrden,
                Item = s.Item,
                IdSolicitante = s.IdSolicitante,
                IdDepositoOrigen = s.IdDepositoOrigen,
                IdProveedor = s.IdProveedor,
                IdDepositoDestino = s.IdDepositoDestino,
                IdVehiculo = s.IdVehiculo,
                IdResiduo = s.IdResiduo,
                IdMaterial = s.IdMaterial,
                Descripcion = s.Descripcion,
                IdTratamiento = s.IdTratamiento,
                FechaInicio = plan?.FechaInicio ?? s.FechaInicioDetalle ?? s.FechaSolicitud,
                IdResponsable = plan?.IdResponsable,
                IdResponsable2 = plan?.IdResponsable2,
                CantidadOrden = s.CantidadSolicitud,
                PesoOrden = s.PesoSolicitud,
                VolumenOrden = s.VolumenSolicitud,
                Cantidad = s.Cantidad,
                Peso = s.Peso,
                Volumen = s.Volumen,
                IdEmbalaje = s.IdEmbalaje,
                PrecioCompra = s.PrecioCompra,
                PrecioServicio = s.PrecioServicio,
                IdEstado = s.IdEstado,
                MultiplesGeneradores = s.MultiplesGeneradores,
                IdEtapa = s.IdEtapa,
                IdFase = s.IdFase,
                Soporte = s.Soporte,
                Notas = s.Notas,
                Procesado = s.Procesado,
                IdCausa = s.IdCausa,
                IdGrupo = s.IdGrupo,
                Titulo = s.Titulo,
                Material = s.Material,
                Medicion = s.Medicion,
                PesoUnitario = s.PesoUnitario,
                PrecioUnitario = s.PrecioUnitario,
                PrecioServicioUnitario = s.PrecioServicioUnitario,
                Solicitante = s.Solicitante,
                DireccionOrigen = s.DireccionOrigen,
                LatitudOrigen = s.LatitudOrigen,
                LongitudOrigen = s.LongitudOrigen,
                DireccionDestino = s.DireccionDestino,
                LatitudDestino = s.LatitudDestino,
                LongitudDestino = s.LongitudDestino,
                Proveedor = s.Proveedor,
                DepositoOrigen = s.DepositoOrigen,
                DepositoDestino = s.DepositoDestino,
                Tratamiento = s.Tratamiento,
                Embalaje = s.Embalaje,
                EmbalajeSolicitud = s.EmbalajeSolicitud
            };
        }).ToList();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SolicitudWithDetailsDto>> GetPendientesRecoleccionAsync(
        string accountPersonId,
        long? idServicio = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(accountPersonId))
            return Array.Empty<SolicitudWithDetailsDto>();

        var filter = new SolicitudFilter
        {
            PersonIds = new[] { accountPersonId },
            IdServicio = idServicio ?? _requestFilterDefaults.GetTransportServiceId(),
            Estados = _requestFilterDefaults.GetActiveEstadosForTransport(),
            DateFrom = dateFrom,
            DateTo = dateTo,
            ExcludeRecurring = true
        };
        var solicitudes = await _requestRepository.GetSolicitudesAsync(filter, cancellationToken);
        return solicitudes.Where(WasteManagementRules.IsPendingCollection).ToList();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MobileTransportWasteDto>> GetPendientesRecoleccionWithPlanningAsync(
        string accountPersonId,
        DateTime? date = null,
        string? driverId = null,
        long? idServicio = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(accountPersonId))
            return Array.Empty<MobileTransportWasteDto>();

        var dateFrom = date.HasValue ? date.Value.Date : (DateTime?)null;
        var dateTo = date.HasValue ? date.Value.Date : (DateTime?)null;
        var pendientes = await GetPendientesRecoleccionAsync(accountPersonId, idServicio, dateFrom, dateTo, cancellationToken);
        var list = pendientes.ToList();
        if (list.Count == 0)
            return Array.Empty<MobileTransportWasteDto>();

        var keys = list
            .Where(s => s.IdDepositoOrigen.HasValue && s.IdDepositoOrigen.Value != 0)
            .Select(s => (s.IdSolicitud, s.IdDepositoOrigen!.Value))
            .Distinct()
            .ToList();
        var planning = keys.Count > 0
            ? await _requestRepository.GetOrdenPlanningForSolicitudesAsync(keys, cancellationToken)
            : new List<OrdenPlanningDto>();
        var planningDict = planning.ToDictionary(p => (p.IdSolicitud, p.IdDepositoOrigen));

        var result = list.Select(s =>
        {
            var key = (IdSolicitud: s.IdSolicitud, IdDepositoOrigen: s.IdDepositoOrigen ?? 0L);
            var plan = key.IdDepositoOrigen != 0 && planningDict.TryGetValue(key, out var p) ? p : null;
            return new MobileTransportWasteDto
            {
                IdSolicitud = s.IdSolicitud,
                NumeroSolicitud = s.NumeroSolicitud,
                FechaSolicitud = s.FechaSolicitud,
                Ocurrencia = s.Ocurrencia,
                Recurrencia = s.Recurrencia,
                IdOrden = plan?.IdOrden,
                NumeroOrden = plan?.NumeroOrden,
                Item = s.Item,
                IdSolicitante = s.IdSolicitante,
                IdDepositoOrigen = s.IdDepositoOrigen,
                IdProveedor = s.IdProveedor,
                IdDepositoDestino = s.IdDepositoDestino,
                IdVehiculo = s.IdVehiculo,
                IdResiduo = s.IdResiduo,
                IdMaterial = s.IdMaterial,
                Descripcion = s.Descripcion,
                IdTratamiento = s.IdTratamiento,
                FechaInicio = plan?.FechaInicio ?? s.FechaInicioDetalle ?? s.FechaSolicitud,
                IdResponsable = plan?.IdResponsable,
                IdResponsable2 = plan?.IdResponsable2,
                CantidadOrden = s.CantidadSolicitud,
                PesoOrden = s.PesoSolicitud,
                VolumenOrden = s.VolumenSolicitud,
                Cantidad = s.Cantidad,
                Peso = s.Peso,
                Volumen = s.Volumen,
                IdEmbalaje = s.IdEmbalaje,
                PrecioCompra = s.PrecioCompra,
                PrecioServicio = s.PrecioServicio,
                IdEstado = s.IdEstado,
                MultiplesGeneradores = s.MultiplesGeneradores,
                IdEtapa = s.IdEtapa,
                IdFase = s.IdFase,
                Soporte = s.Soporte,
                Notas = s.Notas,
                Procesado = s.Procesado,
                IdCausa = s.IdCausa,
                IdGrupo = s.IdGrupo,
                Titulo = s.Titulo,
                Material = s.Material,
                Medicion = s.Medicion,
                PesoUnitario = s.PesoUnitario,
                PrecioUnitario = s.PrecioUnitario,
                PrecioServicioUnitario = s.PrecioServicioUnitario,
                Solicitante = s.Solicitante,
                DireccionOrigen = s.DireccionOrigen,
                LatitudOrigen = s.LatitudOrigen,
                LongitudOrigen = s.LongitudOrigen,
                DireccionDestino = s.DireccionDestino,
                LatitudDestino = s.LatitudDestino,
                LongitudDestino = s.LongitudDestino,
                Proveedor = s.Proveedor,
                DepositoOrigen = s.DepositoOrigen,
                DepositoDestino = s.DepositoDestino,
                Tratamiento = s.Tratamiento,
                Embalaje = s.Embalaje,
                EmbalajeSolicitud = s.EmbalajeSolicitud
            };
        }).ToList();

        if (date.HasValue)
            result = result.Where(d => d.FechaInicio.HasValue && d.FechaInicio.Value.Date == date.Value.Date).ToList();
        if (!string.IsNullOrEmpty(driverId))
            result = result.Where(d => d.IdResponsable == driverId || d.IdResponsable2 == driverId).ToList();

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SolicitudWithDetailsDto>> GetPendientesRecepcionAsync(
        string accountPersonId,
        long? idServicio = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(accountPersonId))
            return Array.Empty<SolicitudWithDetailsDto>();

        var filter = new SolicitudFilter
        {
            PersonIds = new[] { accountPersonId },
            IdServicio = idServicio ?? _requestFilterDefaults.GetTransportServiceId(),
            Estados = _requestFilterDefaults.GetActiveEstadosForTransport(),
            DateFrom = dateFrom,
            DateTo = dateTo,
            ExcludeRecurring = true
        };
        var solicitudes = await _requestRepository.GetSolicitudesAsync(filter, cancellationToken);
        return solicitudes.Where(WasteManagementRules.IsPendingReception).ToList();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SolicitudWithDetailsDto>> GetPendientesTratamientoAsync(
        string accountPersonId,
        long? idServicio = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(accountPersonId))
            return Array.Empty<SolicitudWithDetailsDto>();

        var filter = new SolicitudFilter
        {
            PersonIds = new[] { accountPersonId },
            IdServicio = idServicio ?? _requestFilterDefaults.GetTransportServiceId(),
            Estados = _requestFilterDefaults.GetActiveEstadosForTransport(),
            DateFrom = dateFrom,
            DateTo = dateTo,
            ExcludeRecurring = true
        };
        var solicitudes = await _requestRepository.GetSolicitudesAsync(filter, cancellationToken);
        return solicitudes.Where(WasteManagementRules.IsPendingTreatment).ToList();
    }

    /// <inheritdoc />
    public Task<ResiduoNextPlanningDto?> GetNextPlannedActivityForResiduoAsync(
        long idResiduo,
        CancellationToken cancellationToken = default)
    {
        return _requestRepository.GetNextOrdenPlanningForResiduoAsync(idResiduo, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SolicitudWithDetailsDto>> GetPendientesSinActividadPlaneadaAsync(
        string accountPersonId,
        long? idServicio = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(accountPersonId))
            return Array.Empty<SolicitudWithDetailsDto>();

        var filter = new SolicitudFilter
        {
            PersonIds = new[] { accountPersonId },
            IdServicio = idServicio ?? _requestFilterDefaults.GetTransportServiceId(),
            Estados = _requestFilterDefaults.GetActiveEstadosForTransport(),
            DateFrom = dateFrom,
            DateTo = dateTo,
            ExcludeRecurring = true
        };
        var solicitudes = await _requestRepository.GetSolicitudesAsync(filter, cancellationToken);
        var pendientes = solicitudes.Where(s =>
            WasteManagementRules.IsPendingCollection(s) ||
            WasteManagementRules.IsPendingReception(s) ||
            WasteManagementRules.IsPendingTreatment(s)).ToList();

        var keys = pendientes
            .Where(s => s.IdDepositoOrigen.HasValue && s.IdDepositoOrigen.Value != 0)
            .Select(s => (s.IdSolicitud, s.IdDepositoOrigen!.Value))
            .Distinct()
            .ToList();
        var planning = keys.Count > 0
            ? await _requestRepository.GetOrdenPlanningForSolicitudesAsync(keys, cancellationToken)
            : new List<OrdenPlanningDto>();
        var planningSet = planning.Select(p => (IdSolicitud: p.IdSolicitud, IdDepositoOrigen: p.IdDepositoOrigen)).ToHashSet();

        return pendientes.Where(s =>
        {
            var key = (IdSolicitud: s.IdSolicitud, IdDepositoOrigen: s.IdDepositoOrigen ?? 0L);
            return key.IdDepositoOrigen == 0 || !planningSet.Contains(key);
        }).ToList();
    }

    /// <summary>
    /// Gets collection tasks for the mobile app (Recolecciones): only tasks where the driver (IdResponsable) is the given person.
    /// </summary>
    public async Task<IEnumerable<ProcessDto>> GetCollectionsForDriverAsync(
        string personId,
        DateTime? date,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(personId))
            return Array.Empty<ProcessDto>();

        var allData = await GetMobileTransportWasteForAccountAsync(personId, cancellationToken);
        var forDriver = allData.Where(d =>
            d.IdResponsable == personId || d.IdResponsable2 == personId);
        var filtered = date.HasValue
            ? forDriver.Where(d => d.FechaInicio.HasValue && d.FechaInicio.Value.Date == date.Value.Date)
            : forDriver;

        return await MapTransportDataToProcessesAsync(filtered, cancellationToken);
    }

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
                Id = Guid.NewGuid().ToString(),
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
                    Id = Guid.NewGuid().ToString(),
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
                    Id = Guid.NewGuid().ToString(),
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
                    Id = Guid.NewGuid().ToString(),
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
        string processId, 
        string? subProcessId)
    {
        return new TaskDto
        {
            Id = Guid.NewGuid().ToString(),
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

