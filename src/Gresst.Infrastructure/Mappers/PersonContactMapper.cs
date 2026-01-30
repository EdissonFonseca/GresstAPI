using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: PersonContact (Domain/Inglés) ↔ PersonaContacto (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class PersonContactMapper : MapperBase<PersonContact, PersonaContacto>
{
    /// <summary>
    /// BD → Domain: PersonaContacto → PersonContact
    /// </summary>
    public override PersonContact ToDomain(PersonaContacto dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new PersonContact
        {
            // IDs (composite key - generate a Guid for domain)
            Id = Guid.NewGuid().ToString(),
            AccountId = dbEntity.IdCuenta.ToString(),
            
            // Relations - DB IdPersona/IdContacto/IdLocalizacion are string
            PersonId = dbEntity.IdPersona ?? string.Empty,
            ContactId = dbEntity.IdContacto ?? string.Empty,
            RelationshipType = dbEntity.IdRelacion ?? string.Empty,
            LocationId = dbEntity.IdLocalizacion,
            
            // Dates
            StartDate = dbEntity.FechaInicio,
            EndDate = dbEntity.FechaFin,
            
            // Status
            Status = dbEntity.IdEstado,
            RequiresReconciliation = dbEntity.RequiereConciliar,
            SendEmail = dbEntity.EnviarCorreo,
            
            // Contact Information (can override Person properties)
            Email = dbEntity.Correo,
            Phone = dbEntity.Telefono,
            Phone2 = dbEntity.Telefono2,
            Address = dbEntity.Direccion,
            Name = dbEntity.Nombre,
            JobTitle = dbEntity.Cargo,
            WebPage = dbEntity.Pagina,
            Signature = dbEntity.Firma,
            Notes = dbEntity.Notas,
            AdditionalData = dbEntity.DatosAdicionales,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: PersonContact → PersonaContacto
    /// </summary>
    public override PersonaContacto ToDatabase(PersonContact domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new PersonaContacto
        {
            // IDs (composite key)
            IdPersona = domainEntity.PersonId ?? string.Empty,
            IdContacto = domainEntity.ContactId ?? string.Empty,
            IdRelacion = domainEntity.RelationshipType,
            IdCuenta = long.TryParse(domainEntity.AccountId, out var pcAc) ? pcAc : 0,
            
            // Dates
            FechaInicio = domainEntity.StartDate,
            FechaFin = domainEntity.EndDate,
            
            // Status
            IdEstado = domainEntity.Status,
            RequiereConciliar = domainEntity.RequiresReconciliation,
            EnviarCorreo = domainEntity.SendEmail,
            
            // Contact Information
            Correo = domainEntity.Email,
            Telefono = domainEntity.Phone,
            Telefono2 = domainEntity.Phone2,
            Direccion = domainEntity.Address,
            Nombre = domainEntity.Name,
            Cargo = domainEntity.JobTitle,
            Pagina = domainEntity.WebPage,
            Firma = domainEntity.Signature,
            IdLocalizacion = domainEntity.LocationId,
            Notas = domainEntity.Notes,
            DatosAdicionales = domainEntity.AdditionalData,
            
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

    public override void UpdateDatabase(PersonContact domainEntity, PersonaContacto dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields (IDs are part of composite key, so don't update them)
        dbEntity.IdRelacion = domainEntity.RelationshipType;
        
        // Dates
        dbEntity.FechaInicio = domainEntity.StartDate;
        dbEntity.FechaFin = domainEntity.EndDate;
        
        // Status
        dbEntity.IdEstado = domainEntity.Status;
        dbEntity.RequiereConciliar = domainEntity.RequiresReconciliation;
        dbEntity.EnviarCorreo = domainEntity.SendEmail;
        
        // Contact Information
        dbEntity.Correo = domainEntity.Email;
        dbEntity.Telefono = domainEntity.Phone;
        dbEntity.Telefono2 = domainEntity.Phone2;
        dbEntity.Direccion = domainEntity.Address;
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.Cargo = domainEntity.JobTitle;
        dbEntity.Pagina = domainEntity.WebPage;
        dbEntity.Firma = domainEntity.Signature;
        dbEntity.IdLocalizacion = domainEntity.LocationId;
        dbEntity.Notas = domainEntity.Notes;
        dbEntity.DatosAdicionales = domainEntity.AdditionalData;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

