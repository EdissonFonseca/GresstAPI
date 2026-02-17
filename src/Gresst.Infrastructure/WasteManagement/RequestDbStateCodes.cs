using Gresst.Domain.Enums;

namespace Gresst.Infrastructure.WasteManagement;

/// <summary>
/// Persistence-specific codes for Solicitud and SolicitudDetalle (IdEstado, IdEtapa, IdFase).
/// Only Infrastructure should reference these; Application uses process enums (RequestFlowStage, RequestFlowPhase).
/// </summary>
public static class RequestDbStateCodes
{
    // Solicitud.IdEstado
    public static class SolicitudEstado
    {
        public const string Pendiente = "M";
        public const string Aprobada = "A";
        public const string Rechazada = "R";
        public const string EnProceso = "T";
        public const string Cancelada = "C";
    }

    // SolicitudDetalle.IdEtapa
    public static class SolicitudEtapa
    {
        public const string Inicio = "I";
        public const string Validacion = "V";
        public const string Transporte = "T";
        public const string Recepcion = "R";
        public const string Procesamiento = "P";
        public const string Finalizacion = "F";
    }

    // SolicitudDetalle.IdFase
    public static class SolicitudFase
    {
        public const string Inicio = "I";
        public const string Planeacion = "P";
        public const string Ejecucion = "E";
        public const string Certificacion = "C";
        public const string Finalizacion = "F";
    }

    public static readonly string[] EstadosActivosTransporte =
        { SolicitudEstado.Pendiente, SolicitudEstado.Aprobada, SolicitudEstado.Rechazada, SolicitudEstado.EnProceso };

    public const long IdServicioTransporteMovil = 8;

    public static RequestFlowStage? ToRequestFlowStage(string? idEtapa)
    {
        if (string.IsNullOrEmpty(idEtapa)) return null;
        return idEtapa.ToUpperInvariant() switch
        {
            "I" => RequestFlowStage.Initial,
            "V" => RequestFlowStage.Validation,
            "T" => RequestFlowStage.Transport,
            "R" => RequestFlowStage.Reception,
            "P" => RequestFlowStage.Processing,
            "F" => RequestFlowStage.Finalization,
            _ => null
        };
    }

    public static RequestFlowPhase? ToRequestFlowPhase(string? idFase)
    {
        if (string.IsNullOrEmpty(idFase)) return null;
        return idFase.ToUpperInvariant() switch
        {
            "I" => RequestFlowPhase.Initial,
            "P" => RequestFlowPhase.Planning,
            "E" => RequestFlowPhase.Execution,
            "C" => RequestFlowPhase.Certification,
            "F" => RequestFlowPhase.Finalization,
            _ => null
        };
    }
}
