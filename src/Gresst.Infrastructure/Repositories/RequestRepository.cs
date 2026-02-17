using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Enums;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Data.Entities;
using Gresst.Infrastructure.Mappers;
using Gresst.Infrastructure.WasteManagement;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for operations related to Requests (Solicitudes).
/// Returns only current state; no join with Orden/OrdenPlaneacion (planning is applied in the application layer).
/// </summary>
public class RequestRepository : IRequestRepository
{
    private readonly InfrastructureDbContext _context;

    public RequestRepository(InfrastructureDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SolicitudWithDetailsDto>> GetSolicitudesAsync(
        SolicitudFilter filter,
        CancellationToken cancellationToken = default)
    {
        var personIds = filter.PersonIds?.Where(id => !string.IsNullOrEmpty(id)).ToList() ?? new List<string>();
        var hasPersonFilter = personIds.Count > 0;
        var estados = filter.Estados?.Where(e => !string.IsNullOrEmpty(e)).ToList() ?? new List<string>();
        var hasEstadoFilter = estados.Count > 0;

        var results = new List<SolicitudWithDetailsDto>();

        // Part 1: Solicitud with SolicitudDetalle
        var query1 = from s in _context.Solicituds
                     join sd in _context.SolicitudDetalles on s.IdSolicitud equals sd.IdSolicitud
                     join m in _context.Materials on sd.IdMaterial equals m.IdMaterial
                     join ps in _context.Personas on sd.IdSolicitante equals ps.IdPersona into psGroup
                     from ps in psGroup.DefaultIfEmpty()
                     join doDep in _context.Depositos on sd.IdDepositoOrigen equals doDep.IdDeposito into doGroup
                     from doDep in doGroup.DefaultIfEmpty()
                     join pp in _context.Personas on sd.IdProveedor equals pp.IdPersona into ppGroup
                     from pp in ppGroup.DefaultIfEmpty()
                     join ddDep in _context.Depositos on sd.IdDepositoDestino equals ddDep.IdDeposito into ddGroup
                     from ddDep in ddGroup.DefaultIfEmpty()
                     join trt in _context.Tratamientos on sd.IdTratamiento equals trt.IdTratamiento into trtGroup
                     from trt in trtGroup.DefaultIfEmpty()
                     join e in _context.Embalajes on sd.IdEmbalajeSolicitud equals e.IdEmbalaje into eGroup
                     from e in eGroup.DefaultIfEmpty()
                     join em in _context.Embalajes on sd.IdEmbalaje equals em.IdEmbalaje into emGroup
                     from em in emGroup.DefaultIfEmpty()
                     where (!hasPersonFilter || personIds.Contains(s.IdPersona) || (s.IdTransportador != null && personIds.Contains(s.IdTransportador!)))
                         && (!filter.IdServicio.HasValue || s.IdServicio == filter.IdServicio.Value)
                         && (!hasEstadoFilter || (s.IdEstado != null && estados.Contains(s.IdEstado)))
                         && (!filter.ExcludeRecurring || s.Recurrencia == null || s.Recurrencia == "")
                         && (!filter.DateFrom.HasValue || s.FechaInicio >= filter.DateFrom.Value.Date)
                         && (!filter.DateTo.HasValue || s.FechaInicio <= filter.DateTo.Value.Date)
                     select new
                     {
                         Solicitud = s,
                         SolicitudDetalle = sd,
                         Material = m,
                         Solicitante = ps,
                         DepositoOrigen = doDep,
                         Proveedor = pp,
                         DepositoDestino = ddDep,
                         Tratamiento = trt,
                         EmbalajeSolicitud = e,
                         Embalaje = em
                     };

        var data1 = await query1.ToListAsync(cancellationToken);

        var materialIds = data1.Select(d => d.SolicitudDetalle.IdMaterial).Distinct().ToList();
        var personIdForPricing = hasPersonFilter ? personIds.First() : null;
        var personaMaterials = !string.IsNullOrEmpty(personIdForPricing) && materialIds.Count > 0
            ? await _context.PersonaMaterials
                .Where(pm => pm.IdPersona == personIdForPricing && materialIds.Contains(pm.IdMaterial))
                .ToListAsync(cancellationToken)
            : new List<PersonaMaterial>();
        var personaMaterialDict = personaMaterials.ToDictionary(pm => pm.IdMaterial);

        foreach (var d in data1)
        {
            var personaMaterial = personaMaterialDict.ContainsKey(d.SolicitudDetalle.IdMaterial) ? personaMaterialDict[d.SolicitudDetalle.IdMaterial] : null;
            results.Add(MapToSolicitudWithDetails(
                d.Solicitud,
                d.SolicitudDetalle,
                d.Material,
                d.Solicitante,
                d.DepositoOrigen,
                d.Proveedor,
                d.DepositoDestino,
                d.Tratamiento,
                d.EmbalajeSolicitud,
                d.Embalaje,
                personaMaterial));
        }

        // Part 2: Solicitud without SolicitudDetalle
        var solicitudesConDetalle = await _context.SolicitudDetalles
            .Select(sd => sd.IdSolicitud)
            .Distinct()
            .ToListAsync(cancellationToken);

        var query2 = from s in _context.Solicituds
                     join ps in _context.Personas on s.IdSolicitante equals ps.IdPersona into psGroup
                     from ps in psGroup.DefaultIfEmpty()
                     join doDep in _context.Depositos on s.IdDepositoOrigen equals doDep.IdDeposito into doGroup
                     from doDep in doGroup.DefaultIfEmpty()
                     join pp in _context.Personas on s.IdProveedor equals pp.IdPersona into ppGroup
                     from pp in ppGroup.DefaultIfEmpty()
                     join ddDep in _context.Depositos on s.IdDepositoDestino equals ddDep.IdDeposito into ddGroup
                     from ddDep in ddGroup.DefaultIfEmpty()
                     where (!hasPersonFilter || personIds.Contains(s.IdPersona) || (s.IdTransportador != null && personIds.Contains(s.IdTransportador!)))
                         && (!filter.IdServicio.HasValue || s.IdServicio == filter.IdServicio.Value)
                         && (!hasEstadoFilter || (s.IdEstado != null && estados.Contains(s.IdEstado)))
                         && (!filter.ExcludeRecurring || s.Recurrencia == null || s.Recurrencia == "")
                         && (!filter.DateFrom.HasValue || s.FechaInicio >= filter.DateFrom.Value.Date)
                         && (!filter.DateTo.HasValue || s.FechaInicio <= filter.DateTo.Value.Date)
                         && !solicitudesConDetalle.Contains(s.IdSolicitud)
                     select new { Solicitud = s, Solicitante = ps, DepositoOrigen = doDep, Proveedor = pp, DepositoDestino = ddDep };

        var data2 = await query2.ToListAsync(cancellationToken);

        foreach (var d in data2)
        {
            results.Add(MapToSolicitudWithDetailsNoDetail(d.Solicitud, d.Solicitante, d.DepositoOrigen, d.Proveedor, d.DepositoDestino));
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OrdenPlanningDto>> GetOrdenPlanningForSolicitudesAsync(
        IReadOnlyList<(long IdSolicitud, long IdDepositoOrigen)> keys,
        CancellationToken cancellationToken = default)
    {
        if (keys.Count == 0)
            return Array.Empty<OrdenPlanningDto>();

        var solicitudIds = keys.Select(k => k.IdSolicitud).Distinct().ToList();
        var depositoIds = keys.Select(k => k.IdDepositoOrigen).Distinct().ToList();

        var maxOrden = await (from op in _context.OrdenPlaneacions
                              where solicitudIds.Contains(op.IdSolicitud) && depositoIds.Contains(op.IdDeposito)
                              group op by new { op.IdSolicitud, op.IdDeposito } into g
                              select new
                              {
                                  g.Key.IdSolicitud,
                                  g.Key.IdDeposito,
                                  MaxIdOrden = g.Max(x => x.IdOrden)
                              }).ToListAsync(cancellationToken);

        var keySet = keys.Select(k => (k.IdSolicitud, k.IdDepositoOrigen)).ToHashSet();
        maxOrden = maxOrden.Where(x => keySet.Contains((x.IdSolicitud, x.IdDeposito))).ToList();

        var ordenIds = maxOrden.Select(x => x.MaxIdOrden).Distinct().ToList();
        var ordenes = await _context.Ordens
            .Where(o => ordenIds.Contains(o.IdOrden))
            .ToListAsync(cancellationToken);
        var ordenDict = ordenes.ToDictionary(o => o.IdOrden);

        return maxOrden
            .Select(x =>
            {
                var orden = ordenDict.GetValueOrDefault(x.MaxIdOrden);
                return new OrdenPlanningDto
                {
                    IdSolicitud = x.IdSolicitud,
                    IdDepositoOrigen = x.IdDeposito,
                    IdOrden = x.MaxIdOrden,
                    NumeroOrden = orden?.NumeroOrden,
                    IdResponsable = orden?.IdResponsable,
                    IdResponsable2 = orden?.IdResponsable2,
                    FechaInicio = orden?.FechaInicio
                };
            })
            .ToList();
    }

    /// <inheritdoc />
    public async Task<ResiduoNextPlanningDto?> GetNextOrdenPlanningForResiduoAsync(
        long idResiduo,
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var next = await (from or in _context.OrdenResiduos
                          join o in _context.Ordens on or.IdOrden equals o.IdOrden
                          where or.IdResiduo == idResiduo
                                && (o.FechaFin == null || o.FechaFin >= today)
                                && o.FechaInicio >= today
                          orderby o.FechaInicio
                          select new ResiduoNextPlanningDto
                          {
                              IdResiduo = idResiduo,
                              IdOrden = o.IdOrden,
                              NumeroOrden = o.NumeroOrden,
                              FechaInicio = o.FechaInicio,
                              FechaFin = o.FechaFin,
                              IdTratamiento = o.IdTratamiento ?? or.IdTratamiento,
                              IdResponsable = o.IdResponsable
                          })
            .FirstOrDefaultAsync(cancellationToken);

        if (next != null)
            return next;

        // If no future order, return the first open order (FechaFin null) regardless of FechaInicio
        return await (from or in _context.OrdenResiduos
                      join o in _context.Ordens on or.IdOrden equals o.IdOrden
                      where or.IdResiduo == idResiduo && o.FechaFin == null
                      orderby o.FechaInicio
                      select new ResiduoNextPlanningDto
                      {
                          IdResiduo = idResiduo,
                          IdOrden = o.IdOrden,
                          NumeroOrden = o.NumeroOrden,
                          FechaInicio = o.FechaInicio,
                          FechaFin = o.FechaFin,
                          IdTratamiento = o.IdTratamiento ?? or.IdTratamiento,
                          IdResponsable = o.IdResponsable
                      })
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static SolicitudWithDetailsDto MapToSolicitudWithDetails(
        Solicitud s,
        SolicitudDetalle sd,
        Material m,
        Persona? ps,
        Deposito? doDep,
        Persona? pp,
        Deposito? ddDep,
        Tratamiento? trt,
        Embalaje? eSolicitud,
        Embalaje? em,
        PersonaMaterial? personaMaterial)
    {
        return new SolicitudWithDetailsDto
        {
            IdSolicitud = s.IdSolicitud,
            NumeroSolicitud = s.NumeroSolicitud,
            FechaSolicitud = s.FechaInicio,
            Ocurrencia = s.Ocurrencia,
            Recurrencia = s.Recurrencia,
            IdEstado = s.IdEstado ?? "",
            MultiplesGeneradores = s.MultiplesGeneradores,
            Item = sd.Item,
            IdSolicitante = sd.IdSolicitante,
            IdDepositoOrigen = sd.IdDepositoOrigen,
            IdProveedor = sd.IdProveedor,
            IdDepositoDestino = sd.IdDepositoDestino,
            IdVehiculo = sd.IdVehiculo,
            IdResiduo = sd.IdResiduo,
            IdMaterial = sd.IdMaterial,
            Descripcion = sd.Descripcion,
            IdTratamiento = sd.IdTratamiento,
            FechaInicioDetalle = sd.FechaInicio,
            CantidadSolicitud = sd.CantidadSolicitud,
            PesoSolicitud = sd.PesoSolicitud,
            VolumenSolicitud = sd.VolumenSolicitud,
            Cantidad = sd.Cantidad ?? sd.CantidadSolicitud,
            Peso = sd.Peso ?? sd.PesoSolicitud,
            Volumen = sd.Volumen ?? sd.VolumenSolicitud,
            IdEmbalaje = sd.IdEmbalaje,
            PrecioCompra = sd.PrecioCompra,
            PrecioServicio = sd.PrecioServicio,
            IdEtapa = sd.IdEtapa,
            IdFase = sd.IdFase,
            Stage = RequestDbStateCodes.ToRequestFlowStage(sd.IdEtapa),
            Phase = RequestDbStateCodes.ToRequestFlowPhase(sd.IdFase),
            Soporte = sd.Soporte,
            Notas = sd.Notas,
            Procesado = sd.Procesado,
            IdCausa = sd.IdCausa?.ToString(),
            IdGrupo = $"{s.IdSolicitud}-{sd.IdDepositoOrigen ?? 0}",
            Titulo = $"{s.NumeroSolicitud}-{ps?.Nombre ?? "Sin solicitante ..."}-{doDep?.Nombre ?? "Sin punto de recepción ..."}",
            Material = m.Nombre,
            Medicion = m.Medicion,
            PesoUnitario = personaMaterial?.Peso ?? m.Peso,
            PrecioUnitario = personaMaterial?.PrecioCompra ?? m.PrecioCompra,
            PrecioServicioUnitario = personaMaterial?.PrecioServicio ?? m.PrecioServicio,
            Solicitante = ps?.Nombre,
            DireccionOrigen = doDep?.Direccion,
            LatitudOrigen = doDep?.Ubicacion != null ? (double?)doDep.Ubicacion.GetLatitude() : null,
            LongitudOrigen = doDep?.Ubicacion != null ? (double?)doDep.Ubicacion.GetLongitude() : null,
            DireccionDestino = ddDep?.Direccion,
            LatitudDestino = ddDep?.Ubicacion != null ? (double?)ddDep.Ubicacion.GetLatitude() : null,
            LongitudDestino = ddDep?.Ubicacion != null ? (double?)ddDep.Ubicacion.GetLongitude() : null,
            Proveedor = pp?.Nombre,
            DepositoOrigen = doDep?.Nombre ?? "Sin punto de recolección ...",
            DepositoDestino = ddDep?.Nombre ?? "Sin punto de recepción ...",
            Tratamiento = trt?.Nombre,
            Embalaje = em?.Nombre,
            EmbalajeSolicitud = eSolicitud?.Nombre
        };
    }

    private static SolicitudWithDetailsDto MapToSolicitudWithDetailsNoDetail(
        Solicitud s,
        Persona? ps,
        Deposito? doDep,
        Persona? pp,
        Deposito? ddDep)
    {
        return new SolicitudWithDetailsDto
        {
            IdSolicitud = s.IdSolicitud,
            NumeroSolicitud = s.NumeroSolicitud,
            FechaSolicitud = s.FechaInicio,
            Ocurrencia = s.Ocurrencia,
            Recurrencia = s.Recurrencia,
            IdEstado = s.IdEstado ?? "A",
            MultiplesGeneradores = s.MultiplesGeneradores,
            Item = 0,
            IdSolicitante = s.IdSolicitante,
            IdDepositoOrigen = s.IdDepositoOrigen,
            IdProveedor = s.IdProveedor,
            IdDepositoDestino = s.IdDepositoDestino,
            IdVehiculo = s.IdVehiculo,
            IdResiduo = null,
            IdMaterial = null,
            Descripcion = null,
            IdTratamiento = null,
            FechaInicioDetalle = s.FechaInicio,
            CantidadSolicitud = null,
            PesoSolicitud = null,
            VolumenSolicitud = null,
            Cantidad = null,
            Peso = null,
            Volumen = null,
            IdEmbalaje = null,
            PrecioCompra = null,
            PrecioServicio = null,
            IdEtapa = RequestDbStateCodes.SolicitudEtapa.Transporte,
            IdFase = RequestDbStateCodes.SolicitudFase.Ejecucion,
            Stage = RequestFlowStage.Transport,
            Phase = RequestFlowPhase.Execution,
            Soporte = null,
            Notas = null,
            Procesado = null,
            IdCausa = null,
            IdGrupo = $"{s.IdSolicitud}-{s.IdDepositoOrigen ?? 0}",
            Titulo = $"{s.NumeroSolicitud}-{ps?.Nombre ?? "Sin solicitante ..."}-{doDep?.Nombre ?? "Sin punto de recepción ..."}",
            Material = null,
            Medicion = null,
            PesoUnitario = null,
            PrecioUnitario = null,
            PrecioServicioUnitario = null,
            Solicitante = ps?.Nombre,
            DireccionOrigen = doDep?.Direccion,
            LatitudOrigen = doDep?.Ubicacion != null ? (double?)doDep.Ubicacion.GetLatitude() : null,
            LongitudOrigen = doDep?.Ubicacion != null ? (double?)doDep.Ubicacion.GetLongitude() : null,
            DireccionDestino = ddDep?.Direccion,
            LatitudDestino = ddDep?.Ubicacion != null ? (double?)ddDep.Ubicacion.GetLatitude() : null,
            LongitudDestino = ddDep?.Ubicacion != null ? (double?)ddDep.Ubicacion.GetLongitude() : null,
            Proveedor = pp?.Nombre,
            DepositoOrigen = doDep?.Nombre ?? "Sin punto de recolección ...",
            DepositoDestino = ddDep?.Nombre ?? "Sin punto de recepción ...",
            Tratamiento = null,
            Embalaje = null,
            EmbalajeSolicitud = null
        };
    }
}
