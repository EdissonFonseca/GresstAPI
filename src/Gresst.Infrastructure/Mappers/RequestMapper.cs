using Gresst.Domain.Entities;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers
{
    public class RequestMapper : MapperBase<Request, Solicitud>
    {
        private readonly RequestItemMapper _itemMapper;

        public RequestMapper(RequestItemMapper itemMapper)
        {
            _itemMapper = itemMapper;
        }

        public override Request ToDomain(Solicitud dbEntity)
        {
            if (dbEntity == null) 
                throw new ArgumentNullException(nameof(dbEntity));

            return new Request
            {
                Id = dbEntity.IdSolicitud.ToString(),
                RequestNumber = dbEntity.NumeroSolicitud?.ToString() ?? "",
                RequesterId = dbEntity.IdSolicitante ?? "",
                HaulerId = dbEntity.IdTransportador,
                ProviderId = dbEntity.IdProveedor,
                //ServiceId = dbEntity.IdServicio.ToString(),
                RequestedDate = dbEntity.FechaInicio,
                RequiredByDate = dbEntity.FechaInicio,
                IsRecurrency = dbEntity.Recurrencia != null,
                DestinationFacilityId = dbEntity.IdDepositoDestino?.ToString(),
                SourceFacilityId = dbEntity.IdDepositoOrigen?.ToString(),
                VehicleId = dbEntity.IdVehiculo?.ToString(),
                Items = dbEntity.SolicitudDetalles
                    .Select(_itemMapper.ToDomain)
                    .ToList()
            };
            throw new NotImplementedException();
        }
        public override Solicitud ToDatabase(Request domainEntity)
        {
            throw new NotImplementedException();
        }
        public override void UpdateDatabase(Request domainEntity, Solicitud dbEntity)
        {
            throw new NotImplementedException();
        }
    }
}
