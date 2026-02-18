using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gresst.Infrastructure.Mappers
{
    public class RequestItemMapper : MapperBase<RequestItem, SolicitudDetalle>
    {
        public override RequestItem ToDomain(SolicitudDetalle dbEntity)
        {
            if (dbEntity == null)
                throw new ArgumentNullException(nameof(dbEntity));

            RequestItemStatus status = RequestItemStatus.Pending;
            switch (dbEntity.IdEtapa)
            {
                case DBConstants.Estado.Activo:
                    status = RequestItemStatus.Pending;
                    break;
            }
                    
            return new RequestItem
            {
                Id = dbEntity.Item.ToString(),
                Status = status
            };
            throw new NotImplementedException();
        }
        public override SolicitudDetalle ToDatabase(RequestItem domainEntity)
        {
            throw new NotImplementedException();
        }
        public override void UpdateDatabase(RequestItem domainEntity, SolicitudDetalle dbEntity)
        {
            throw new NotImplementedException();
        }
    }
}
