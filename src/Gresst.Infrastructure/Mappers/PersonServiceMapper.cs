using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: PersonService (Domain/Inglés) ↔ PersonaServicio (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class PersonServiceMapper : MapperBase<PersonService, PersonaServicio>
{
    /// <summary>
    /// BD → Domain: PersonaServicio → PersonService
    /// </summary>
    public override PersonService ToDomain(PersonaServicio dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new PersonService
        {
            // IDs (composite key - generate a Guid for domain)
            Id = Guid.NewGuid().ToString(),
            AccountId = dbEntity.IdCuenta.ToString(),
            
            // Relations
            PersonId = GuidStringConverter.ToGuid(dbEntity.IdPersona),
            ServiceId = GuidLongConverter.ToGuid(dbEntity.IdServicio),
            
            // Dates
            StartDate = dbEntity.FechaInicio.ToDateTime(TimeOnly.MinValue),
            EndDate = dbEntity.FechaFin?.ToDateTime(TimeOnly.MinValue),
            
            // Status
            IsActive = dbEntity.Activo,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString()
        };
    }

    /// <summary>
    /// Domain → BD: PersonService → PersonaServicio
    /// </summary>
    public override PersonaServicio ToDatabase(PersonService domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new PersonaServicio
        {
            // IDs (composite key)
            IdPersona = GuidStringConverter.ToString(domainEntity.PersonId),
            IdServicio = GuidLongConverter.ToLong(domainEntity.ServiceId),
            FechaInicio = DateOnly.FromDateTime(domainEntity.StartDate),
            IdCuenta = long.TryParse(domainEntity.AccountId, out var psvAc) ? psvAc : 0,
            
            // Dates
            FechaFin = domainEntity.EndDate.HasValue 
                ? DateOnly.FromDateTime(domainEntity.EndDate.Value) 
                : null,
            
            // Status
            Activo = domainEntity.IsActive,
            
            // Audit
            FechaCreacion = domainEntity.CreatedAt,
            IdUsuarioCreacion = !string.IsNullOrEmpty(domainEntity.CreatedBy) 
                ? long.Parse(domainEntity.CreatedBy) 
                : 0,
            FechaUltimaModificacion = domainEntity.UpdatedAt,
            IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
                ? long.Parse(domainEntity.UpdatedBy) 
                : null
        };
    }

    public override void UpdateDatabase(PersonService domainEntity, PersonaServicio dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields (IDs are part of composite key, so don't update them)
        dbEntity.FechaInicio = DateOnly.FromDateTime(domainEntity.StartDate);
        dbEntity.FechaFin = domainEntity.EndDate.HasValue 
            ? DateOnly.FromDateTime(domainEntity.EndDate.Value) 
            : null;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

