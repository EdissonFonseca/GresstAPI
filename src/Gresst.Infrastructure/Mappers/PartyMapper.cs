using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data.Entities;
using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Mappers;

public class PersonaDb
{
    public string IdPersona { get; set; } = String.Empty;
    public string? IdTipoIdentificacion { get; set; }
    public string? IdTipoPersona { get; set; }
    public string? Identificacion { get; set; }
    public int? DigitoVerificacion { get; set; }
    public string? Nombre { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Telefono2 { get; set; }
    public string? Correo { get; set; }
    public List<string>? Relaciones { get; set; }
    public string? IdRelacion { get; set; }
    public bool Activo { get; set; }
    public string? Firma { get; set; }
    public string? IdLocalizacion { get; set; }
    public Geometry? Ubicacion { get; set; }

    public static PersonaDb FromPersona(Persona p) => new()
    {
        IdPersona = p.IdPersona,
        IdTipoIdentificacion = p.IdTipoIdentificacion,
        IdTipoPersona = p.IdTipoPersona,
        Identificacion = p.Identificacion,
        DigitoVerificacion = p.DigitoVerificacion,
        Nombre = p.Nombre,
        Direccion = p.Direccion,
        Telefono = p.Telefono,
        Telefono2 = p.Telefono2,
        Correo = p.Correo,
        Activo = p.Activo,
        Firma = p.Firma,
        IdLocalizacion = p.IdLocalizacion,
        Ubicacion = p.UbicacionMapa,
    };
    public Persona ToPersona() => new()
    {
        IdPersona = IdPersona,
        IdTipoIdentificacion = IdTipoIdentificacion,
        IdTipoPersona = IdTipoPersona,
        Identificacion = Identificacion,
        DigitoVerificacion = DigitoVerificacion,
        Nombre = Nombre,
        Direccion = Direccion,
        Telefono = Telefono,
        Telefono2 = Telefono2,
        Correo = Correo,
        Activo = Activo,
        Firma = Firma,
        IdLocalizacion = IdLocalizacion,
        UbicacionMapa = Ubicacion,
    };

}

public class PartyMapper : MapperBase<Party, PersonaDb>
{
    private readonly ICurrentUserService _currentUserService;

    public PartyMapper(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override Party ToDomain(PersonaDb dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        try
        {
            return new Party
            {
                Id = dbEntity.IdPersona ?? string.Empty,
                Name = dbEntity.Nombre ?? string.Empty,
                DocumentNumber = dbEntity.Identificacion,
                DocumentType = TypeMapper.ToDocumentType(dbEntity.IdTipoIdentificacion ?? ""), 
                PersonType = TypeMapper.ToPersonType(dbEntity.IdTipoPersona ?? ""),
                CheckDigit = dbEntity.DigitoVerificacion,
                Address = dbEntity.Direccion,
                Email = dbEntity.Correo,
                Phone = dbEntity.Telefono,
                SignatureUrl = dbEntity.Firma,
                LocalityId = dbEntity.IdLocalizacion,
                Location = dbEntity.Ubicacion != null ? new Point(dbEntity.Ubicacion.Coordinate) : null,
                IsActive = dbEntity.Activo,
                Relations = (dbEntity.Relaciones ?? new List<string> { dbEntity.IdRelacion ?? string.Empty })
                    .Select(rol => TypeMapper.ToPartyRelationType(rol.Trim()))
                    .Distinct()
                    .ToList(),
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
    public override PersonaDb ToDatabase(Party domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new PersonaDb
        {
            // IDs
            IdPersona = domainEntity.Id,
            Nombre = domainEntity.Name,
            Identificacion = domainEntity.DocumentNumber,
            DigitoVerificacion = domainEntity.CheckDigit,
            IdTipoIdentificacion = domainEntity.DocumentType.HasValue ? TypeMapper.ToTipoIdentificacion(domainEntity.DocumentType.Value) : null,
            IdTipoPersona = domainEntity.PersonType.HasValue ? TypeMapper.ToTipoPersona(domainEntity.PersonType.Value) : null,
            Direccion = domainEntity.Address,
            Correo = domainEntity.Email,
            Telefono = domainEntity.Phone,
            Activo = domainEntity.IsActive,
            Firma = domainEntity.SignatureUrl,
            Ubicacion = domainEntity.Location != null ? new Point(domainEntity.Location.X, domainEntity.Location.Y) : null,
            IdLocalizacion = domainEntity.LocalityId,
            Relaciones = domainEntity.Relations
                .Select(r => TypeMapper.ToTipoRelacion(r))
                .ToList()
        };
    }

    public override void UpdateDatabase(Party domainEntity, PersonaDb dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;
        
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.Identificacion = domainEntity.DocumentNumber;
        dbEntity.DigitoVerificacion = domainEntity.CheckDigit;
        dbEntity.IdLocalizacion = domainEntity.LocalityId;
        dbEntity.Correo = domainEntity.Email;
        dbEntity.Telefono = domainEntity.Phone;
        dbEntity.Direccion = domainEntity.Address;
        dbEntity.Firma = domainEntity.SignatureUrl;
        dbEntity.Activo = domainEntity.IsActive;
        dbEntity.Ubicacion = domainEntity.Location != null ? new Point(domainEntity.Location.X, domainEntity.Location.Y) : null;
        dbEntity.IdTipoIdentificacion = domainEntity.DocumentType.HasValue ? TypeMapper.ToTipoIdentificacion(domainEntity.DocumentType.Value) : null;
        dbEntity.IdTipoPersona = domainEntity.PersonType.HasValue ? TypeMapper.ToTipoPersona(domainEntity.PersonType.Value) : null;
        dbEntity.Relaciones = domainEntity.Relations
            .Select(r => TypeMapper.ToTipoRelacion(r))
            .ToList();
    }

}

