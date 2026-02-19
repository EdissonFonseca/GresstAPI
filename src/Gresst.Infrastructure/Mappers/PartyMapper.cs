using Gresst.Domain.Entities;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Person (Domain/Inglés) ↔ Persona (BD/Español)
/// </summary>
public class PartyMapper : MapperBase<Party, Persona>
{
    /// <summary>
    /// BD → Domain: Persona → Person
    /// </summary>
    public override Party ToDomain(Persona dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        try
        {
            return new Party
            {
                // IDs
                Id = dbEntity.IdPersona ?? string.Empty,
                AccountId = dbEntity.IdCuenta?.ToString() ?? string.Empty,

                // Basic Info
                Name = dbEntity.Nombre ?? string.Empty,
                DocumentNumber = dbEntity.Identificacion,
                Email = dbEntity.Correo,
                Phone = dbEntity.Telefono,
                Address = dbEntity.Direccion,

                // Audit
                CreatedAt = dbEntity.FechaCreacion,
                UpdatedAt = dbEntity.FechaUltimaModificacion,
                CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
                UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
                IsActive = dbEntity.Activo
            };
        }
        catch (Exception ex)
        {
            var str = ex.Message;
            return new Party();
        }
    }

    /// <summary>
    /// Domain → BD: Person → Persona
    /// </summary>
    public override Persona ToDatabase(Party domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new Persona
        {
            // IDs
            IdPersona = domainEntity.Id,
            IdCuenta = string.IsNullOrEmpty(domainEntity.AccountId) ? null : long.Parse(domainEntity.AccountId),
            
            // Basic Info
            Nombre = domainEntity.Name,
            Identificacion = domainEntity.DocumentNumber,
            Correo = domainEntity.Email,
            Telefono = domainEntity.Phone,
            Direccion = domainEntity.Address,
            
            // Defaults for required fields
            IdRol = "01", // Default role
            IdTipoPersona = "J", // Jurídica por defecto
            
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

    public override void UpdateDatabase(Party domainEntity, Persona dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.Identificacion = domainEntity.DocumentNumber;
        dbEntity.Correo = domainEntity.Email;
        dbEntity.Telefono = domainEntity.Phone;
        dbEntity.Direccion = domainEntity.Address;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }

}

