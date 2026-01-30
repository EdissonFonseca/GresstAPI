using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Data.Entities;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for operations related to Requests (Solicitudes)
/// </summary>
public class RequestRepository : IRequestRepository
{
    private readonly InfrastructureDbContext _context;

    public RequestRepository(InfrastructureDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets mobile transport waste data implementing the fnResiduosTransporteMovil logic
    /// </summary>
    /// <param name="personId">Person ID (domain string)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of mobile transport waste data</returns>
    public async Task<IEnumerable<MobileTransportWasteDto>> GetMobileTransportWasteAsync(
        string personId, 
        CancellationToken cancellationToken = default)
    {
        var personIdString = personId ?? string.Empty;
        
        if (string.IsNullOrEmpty(personIdString))
        {
            return new List<MobileTransportWasteDto>();
        }

        // Service ID = 8 (mobile transport)
        const long servicioId = 8;

        var results = new List<MobileTransportWasteDto>();

        // First part: Solicitud with SolicitudDetalle
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
                     where (s.IdPersona == personIdString || s.IdTransportador == personIdString)
                         && s.IdServicio == servicioId
                         && (sd.IdEtapa == "T" || sd.IdEtapa == "R" || (sd.IdEtapa == "P" && sd.IdFase == "I") || (sd.IdEtapa == "F" && sd.IdFase == "F"))
                         && (s.IdEstado == "M" || s.IdEstado == "A" || s.IdEstado == "R" || s.IdEstado == "T")
                         && (s.Recurrencia == null || s.Recurrencia == "")
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
                         Embalaje = em,
                         IdDepositoOrigen = sd.IdDepositoOrigen
                     };

        var data1 = await query1.ToListAsync(cancellationToken);

        // Get OrdenPlaneacion data (MAX IdOrden per IdSolicitud and IdDeposito)
        var ordenPlaneacionIds = data1
            .Where(d => d.IdDepositoOrigen.HasValue)
            .Select(d => new { IdSolicitud = d.Solicitud.IdSolicitud, IdDeposito = d.IdDepositoOrigen!.Value })
            .GroupBy(x => new { x.IdSolicitud, x.IdDeposito })
            .Select(g => g.Key)
            .ToList();

        // Get OrdenPlaneacion MAX IdOrden per IdSolicitud and IdDeposito
        var solicitudIds = ordenPlaneacionIds.Select(x => x.IdSolicitud).Distinct().ToList();
        var depositoIds = ordenPlaneacionIds.Select(x => x.IdDeposito).Distinct().ToList();
        
        var ordenPlaneacionMax = await (from op in _context.OrdenPlaneacions
                                       where solicitudIds.Contains(op.IdSolicitud) && depositoIds.Contains(op.IdDeposito)
                                       group op by new { op.IdSolicitud, op.IdDeposito } into g
                                       select new
                                       {
                                           g.Key.IdSolicitud,
                                           g.Key.IdDeposito,
                                           MaxIdOrden = g.Max(x => x.IdOrden)
                                       }).ToListAsync(cancellationToken);
        
        // Filter to only include the ones we need
        ordenPlaneacionMax = ordenPlaneacionMax
            .Where(x => ordenPlaneacionIds.Any(id => id.IdSolicitud == x.IdSolicitud && id.IdDeposito == x.IdDeposito))
            .ToList();

        var ordenPlaneacionDict = ordenPlaneacionMax.ToDictionary(x => new { x.IdSolicitud, x.IdDeposito });

        // Get Orden data
        var ordenIds = ordenPlaneacionMax.Select(x => x.MaxIdOrden).Distinct().ToList();
        var ordenes = await _context.Ordens
            .Where(o => ordenIds.Contains(o.IdOrden))
            .ToListAsync(cancellationToken);
        var ordenDict = ordenes.ToDictionary(o => o.IdOrden);

        // Get PersonaMaterial data
        var materialIds = data1.Select(d => d.SolicitudDetalle.IdMaterial).Distinct().ToList();
        var personaMaterials = await _context.PersonaMaterials
            .Where(pm => pm.IdPersona == personIdString && materialIds.Contains(pm.IdMaterial))
            .ToListAsync(cancellationToken);
        var personaMaterialDict = personaMaterials.ToDictionary(pm => pm.IdMaterial);

        // Get OrdenResiduo data
        var residuoIds = data1.Where(d => d.SolicitudDetalle.IdResiduo.HasValue)
            .Select(d => d.SolicitudDetalle.IdResiduo!.Value)
            .Distinct()
            .ToList();
        var ordenResiduos = await _context.OrdenResiduos
            .Where(or => residuoIds.Contains(or.IdResiduo))
            .ToListAsync(cancellationToken);
        var ordenResiduoDict = ordenResiduos.ToDictionary(or => or.IdResiduo);

        // Map first part results
        foreach (var d in data1)
        {
            var key = new { d.Solicitud.IdSolicitud, IdDeposito = d.IdDepositoOrigen ?? 0 };
            var ordenPlaneacionKey = ordenPlaneacionDict.ContainsKey(key) ? ordenPlaneacionDict[key] : null;
            var orden = ordenPlaneacionKey != null && ordenDict.ContainsKey(ordenPlaneacionKey.MaxIdOrden) 
                ? ordenDict[ordenPlaneacionKey.MaxIdOrden] 
                : null;
            var personaMaterial = personaMaterialDict.ContainsKey(d.SolicitudDetalle.IdMaterial) 
                ? personaMaterialDict[d.SolicitudDetalle.IdMaterial] 
                : null;
            var ordenResiduo = d.SolicitudDetalle.IdResiduo.HasValue && ordenResiduoDict.ContainsKey(d.SolicitudDetalle.IdResiduo.Value)
                ? ordenResiduoDict[d.SolicitudDetalle.IdResiduo.Value]
                : null;

            var fechaInicio = orden?.FechaInicio ?? d.SolicitudDetalle.FechaInicio ?? d.Solicitud.FechaInicio;

            results.Add(new MobileTransportWasteDto
            {
                IdSolicitud = d.Solicitud.IdSolicitud,
                NumeroSolicitud = d.Solicitud.NumeroSolicitud,
                FechaSolicitud = d.Solicitud.FechaInicio,
                Ocurrencia = d.Solicitud.Ocurrencia,
                Recurrencia = d.Solicitud.Recurrencia,
                IdOrden = orden?.IdOrden,
                NumeroOrden = orden?.NumeroOrden,
                Item = d.SolicitudDetalle.Item,
                IdSolicitante = d.SolicitudDetalle.IdSolicitante,
                IdDepositoOrigen = d.SolicitudDetalle.IdDepositoOrigen,
                IdProveedor = d.SolicitudDetalle.IdProveedor,
                IdDepositoDestino = d.SolicitudDetalle.IdDepositoDestino,
                IdVehiculo = d.SolicitudDetalle.IdVehiculo,
                IdResiduo = d.SolicitudDetalle.IdResiduo,
                IdMaterial = d.SolicitudDetalle.IdMaterial,
                Descripcion = d.SolicitudDetalle.Descripcion,
                IdTratamiento = d.SolicitudDetalle.IdTratamiento,
                FechaInicio = fechaInicio.Date,
                IdResponsable = orden?.IdResponsable,
                IdResponsable2 = orden?.IdResponsable2,
                CantidadOrden = d.SolicitudDetalle.CantidadSolicitud,
                PesoOrden = d.SolicitudDetalle.PesoSolicitud,
                VolumenOrden = d.SolicitudDetalle.VolumenSolicitud,
                Cantidad = d.SolicitudDetalle.Cantidad ?? d.SolicitudDetalle.CantidadSolicitud,
                Peso = d.SolicitudDetalle.Peso ?? d.SolicitudDetalle.PesoSolicitud,
                Volumen = d.SolicitudDetalle.Volumen ?? d.SolicitudDetalle.VolumenSolicitud,
                IdEmbalaje = d.SolicitudDetalle.IdEmbalaje,
                PrecioCompra = d.SolicitudDetalle.PrecioCompra,
                PrecioServicio = d.SolicitudDetalle.PrecioServicio,
                IdEstado = d.Solicitud.IdEstado,
                MultiplesGeneradores = d.Solicitud.MultiplesGeneradores,
                IdEtapa = d.SolicitudDetalle.IdEtapa,
                IdFase = d.SolicitudDetalle.IdFase,
                Soporte = d.SolicitudDetalle.Soporte,
                Notas = d.SolicitudDetalle.Notas,
                Procesado = d.SolicitudDetalle.Procesado,
                IdCausa = d.SolicitudDetalle.IdCausa?.ToString(),
                IdGrupo = $"{d.Solicitud.IdSolicitud}-{d.SolicitudDetalle.IdDepositoOrigen ?? 0}",
                Titulo = $"{d.Solicitud.NumeroSolicitud}-{d.Solicitante?.Nombre ?? "Sin solicitante ..."}-{d.DepositoOrigen?.Nombre ?? "Sin punto de recepción ..."}",
                Material = d.Material.Nombre,
                Medicion = d.Material.Medicion,
                PesoUnitario = personaMaterial?.Peso ?? d.Material.Peso,
                PrecioUnitario = personaMaterial?.PrecioCompra ?? d.Material.PrecioCompra,
                PrecioServicioUnitario = personaMaterial?.PrecioServicio ?? d.Material.PrecioServicio,
                Solicitante = d.Solicitante?.Nombre,
                DireccionOrigen = d.DepositoOrigen?.Direccion,
                LatitudOrigen = d.DepositoOrigen?.Ubicacion != null ? (double?)d.DepositoOrigen.Ubicacion.GetLatitude() : null,
                LongitudOrigen = d.DepositoOrigen?.Ubicacion != null ? (double?)d.DepositoOrigen.Ubicacion.GetLongitude() : null,
                DireccionDestino = d.DepositoDestino?.Direccion,
                LatitudDestino = d.DepositoDestino?.Ubicacion != null ? (double?)d.DepositoDestino.Ubicacion.GetLatitude() : null,
                LongitudDestino = d.DepositoDestino?.Ubicacion != null ? (double?)d.DepositoDestino.Ubicacion.GetLongitude() : null,
                Proveedor = d.Proveedor?.Nombre,
                DepositoOrigen = d.DepositoOrigen?.Nombre ?? "Sin punto de recolección ...",
                DepositoDestino = d.DepositoDestino?.Nombre ?? "Sin punto de recepción ...",
                Tratamiento = d.Tratamiento?.Nombre,
                Embalaje = d.Embalaje?.Nombre,
                EmbalajeSolicitud = d.EmbalajeSolicitud?.Nombre
            });
        }

        // Second part: Solicitud without SolicitudDetalle
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
                     where (s.IdPersona == personIdString || s.IdTransportador == personIdString)
                         && s.IdServicio == servicioId
                         && (s.IdEstado == "M" || s.IdEstado == "A" || s.IdEstado == "R" || s.IdEstado == "T")
                         && (s.Recurrencia == null || s.Recurrencia == "")
                         && !solicitudesConDetalle.Contains(s.IdSolicitud)
                     select new
                     {
                         Solicitud = s,
                         Solicitante = ps,
                         DepositoOrigen = doDep,
                         Proveedor = pp,
                         DepositoDestino = ddDep,
                         IdDepositoOrigen = s.IdDepositoOrigen
                     };

        var data2 = await query2.ToListAsync(cancellationToken);

        // Get OrdenPlaneacion for second part
        var ordenPlaneacionIds2 = data2
            .Where(d => d.IdDepositoOrigen.HasValue)
            .Select(d => new { IdSolicitud = d.Solicitud.IdSolicitud, IdDeposito = d.IdDepositoOrigen!.Value })
            .GroupBy(x => new { x.IdSolicitud, x.IdDeposito })
            .Select(g => g.Key)
            .ToList();

        // Get OrdenPlaneacion MAX IdOrden for second part
        var solicitudIds2 = ordenPlaneacionIds2.Select(x => x.IdSolicitud).Distinct().ToList();
        var depositoIds2 = ordenPlaneacionIds2.Select(x => x.IdDeposito).Distinct().ToList();
        
        var ordenPlaneacionMax2 = await (from op in _context.OrdenPlaneacions
                                         where solicitudIds2.Contains(op.IdSolicitud) && depositoIds2.Contains(op.IdDeposito)
                                         group op by new { op.IdSolicitud, op.IdDeposito } into g
                                         select new
                                         {
                                             g.Key.IdSolicitud,
                                             g.Key.IdDeposito,
                                             MaxIdOrden = g.Max(x => x.IdOrden)
                                         }).ToListAsync(cancellationToken);
        
        // Filter to only include the ones we need
        ordenPlaneacionMax2 = ordenPlaneacionMax2
            .Where(x => ordenPlaneacionIds2.Any(id => id.IdSolicitud == x.IdSolicitud && id.IdDeposito == x.IdDeposito))
            .ToList();

        var ordenPlaneacionDict2 = ordenPlaneacionMax2.ToDictionary(x => new { x.IdSolicitud, x.IdDeposito });

        // Get Orden data for second part
        var ordenIds2 = ordenPlaneacionMax2.Select(x => x.MaxIdOrden).Distinct().ToList();
        var ordenes2 = await _context.Ordens
            .Where(o => ordenIds2.Contains(o.IdOrden))
            .ToListAsync(cancellationToken);
        var ordenDict2 = ordenes2.ToDictionary(o => o.IdOrden);

        // Map second part results
        foreach (var d in data2)
        {
            var key = new { d.Solicitud.IdSolicitud, IdDeposito = d.IdDepositoOrigen ?? 0 };
            var ordenPlaneacionKey = ordenPlaneacionDict2.ContainsKey(key) ? ordenPlaneacionDict2[key] : null;
            var orden = ordenPlaneacionKey != null && ordenDict2.ContainsKey(ordenPlaneacionKey.MaxIdOrden)
                ? ordenDict2[ordenPlaneacionKey.MaxIdOrden]
                : null;

            results.Add(new MobileTransportWasteDto
            {
                IdSolicitud = d.Solicitud.IdSolicitud,
                NumeroSolicitud = d.Solicitud.NumeroSolicitud,
                FechaSolicitud = d.Solicitud.FechaInicio,
                Ocurrencia = d.Solicitud.Ocurrencia,
                Recurrencia = d.Solicitud.Recurrencia,
                IdOrden = orden?.IdOrden,
                NumeroOrden = orden?.NumeroOrden,
                Item = 0,
                IdSolicitante = d.Solicitud.IdSolicitante,
                IdDepositoOrigen = d.Solicitud.IdDepositoOrigen,
                IdProveedor = d.Solicitud.IdProveedor,
                IdDepositoDestino = d.Solicitud.IdDepositoDestino,
                IdVehiculo = d.Solicitud.IdVehiculo,
                IdResiduo = null,
                IdMaterial = null,
                Descripcion = null,
                IdTratamiento = null,
                FechaInicio = d.Solicitud.FechaInicio.Date,
                IdResponsable = orden?.IdResponsable,
                IdResponsable2 = orden?.IdResponsable2,
                CantidadOrden = 0,
                PesoOrden = 0,
                VolumenOrden = 0,
                Cantidad = 0,
                Peso = 0,
                Volumen = 0,
                IdEmbalaje = null,
                PrecioCompra = null,
                PrecioServicio = null,
                IdEstado = "A",
                MultiplesGeneradores = d.Solicitud.MultiplesGeneradores,
                IdEtapa = "T",
                IdFase = "E",
                Soporte = null,
                Notas = null,
                Procesado = null,
                IdCausa = null,
                IdGrupo = $"{d.Solicitud.IdSolicitud}-{d.Solicitud.IdDepositoOrigen ?? 0}",
                Titulo = $"{d.Solicitud.NumeroSolicitud}-{d.Solicitante?.Nombre ?? "Sin solicitante ..."}-{d.DepositoOrigen?.Nombre ?? "Sin punto de recepción ..."}",
                Material = null,
                Medicion = null,
                PesoUnitario = null,
                PrecioUnitario = null,
                PrecioServicioUnitario = null,
                Solicitante = d.Solicitante?.Nombre,
                DireccionOrigen = d.DepositoOrigen?.Direccion,
                LatitudOrigen = d.DepositoOrigen?.Ubicacion != null ? (double?)d.DepositoOrigen.Ubicacion.GetLatitude() : null,
                LongitudOrigen = d.DepositoOrigen?.Ubicacion != null ? (double?)d.DepositoOrigen.Ubicacion.GetLongitude() : null,
                DireccionDestino = d.DepositoDestino?.Direccion,
                LatitudDestino = d.DepositoDestino?.Ubicacion != null ? (double?)d.DepositoDestino.Ubicacion.GetLatitude() : null,
                LongitudDestino = d.DepositoDestino?.Ubicacion != null ? (double?)d.DepositoDestino.Ubicacion.GetLongitude() : null,
                Proveedor = d.Proveedor?.Nombre,
                DepositoOrigen = d.DepositoOrigen?.Nombre ?? "Sin punto de recolección ...",
                DepositoDestino = d.DepositoDestino?.Nombre ?? "Sin punto de recepción ...",
                Tratamiento = null,
                Embalaje = null,
                EmbalajeSolicitud = null
            });
        }

        return results;
    }
}
