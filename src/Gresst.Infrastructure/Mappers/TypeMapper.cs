using Gresst.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Gresst.Infrastructure.Mappers
{
    public static class TypeMapper
    {
        public static DocumentType ToDocumentType(string tipoIdentificacion) => tipoIdentificacion switch
        {
            TipoIdentificacion.CI => DocumentType.CI,
            TipoIdentificacion.CJ => DocumentType.CJ,
            TipoIdentificacion.CC => DocumentType.CC,
            TipoIdentificacion.CE => DocumentType.CE,
            TipoIdentificacion.CUIT => DocumentType.CUIT,
            TipoIdentificacion.DNI => DocumentType.DNI,
            TipoIdentificacion.NIT => DocumentType.NIT,
            TipoIdentificacion.Pasaporte => DocumentType.Passport,
            TipoIdentificacion.PersonaJuridica => DocumentType.LegalEntity,
            TipoIdentificacion.RTN => DocumentType.RTN,
            TipoIdentificacion.RUC => DocumentType.RUN,
            TipoIdentificacion.RUT => DocumentType.RUT,
            _ => throw new ArgumentOutOfRangeException(nameof(tipoIdentificacion), $"Unknown tipo identificacion: {tipoIdentificacion}")
        };
        public static string ToTipoIdentificacion(this DocumentType documentType) => documentType switch
        {
            DocumentType.CI => TipoIdentificacion.CI,
            DocumentType.CJ => TipoIdentificacion.CJ,
            DocumentType.CC => TipoIdentificacion.CC,
            DocumentType.CE => TipoIdentificacion.CE,
            DocumentType.CUIT => TipoIdentificacion.CUIT,
            DocumentType.DNI => TipoIdentificacion.DNI,
            DocumentType.NIT => TipoIdentificacion.NIT,
            DocumentType.Passport => TipoIdentificacion.Pasaporte,
            DocumentType.LegalEntity => TipoIdentificacion.PersonaJuridica,
            DocumentType.RTN => TipoIdentificacion.RTN,
            DocumentType.RUN => TipoIdentificacion.RUC,
            DocumentType.RUT => TipoIdentificacion.RUT,
            _ => throw new ArgumentOutOfRangeException(nameof(documentType), $"Unknown document type: {documentType}")
        };
        public static PersonType ToPersonType(string tipoPersona) => tipoPersona switch
        {
            TipoPersona.Natural => PersonType.Individual,
            TipoPersona.Juridica => PersonType.LegalEntity,
            _ => throw new ArgumentOutOfRangeException(nameof(tipoPersona), $"Unknown person type: {tipoPersona}")
        };
        public static string ToTipoPersona(PersonType personType) => personType switch
        {
            PersonType.Individual => TipoPersona.Natural,
            PersonType.LegalEntity => TipoPersona.Juridica,
            _ => throw new ArgumentOutOfRangeException(nameof(personType), $"Unknown person type: {personType}")
        };
        public static PartyRelationType ToPartyRelationType(string relationCode) => relationCode switch
        {
            TipoRelacion.Cliente => PartyRelationType.Customer,
            TipoRelacion.Proveedor => PartyRelationType.Supplier,
            TipoRelacion.Empleado => PartyRelationType.Employee,
            TipoRelacion.Conductor => PartyRelationType.Driver,
            _ => PartyRelationType.Unknown
        };
        public static string ToTipoRelacion(PartyRelationType relationType) => relationType switch
        {
            PartyRelationType.Customer => TipoRelacion.Cliente,
            PartyRelationType.Supplier => TipoRelacion.Proveedor,
            PartyRelationType.Employee => TipoRelacion.Empleado,
            PartyRelationType.Driver => TipoRelacion.Conductor,
            _ => throw new ArgumentOutOfRangeException(nameof(relationType), $"Unknown party relation type: {relationType}")
        };
        public static List<FacilityType> ToFacilityTypes(Deposito d)
        {
            var types = new List<FacilityType>();

            if (d.Recepcion == true) types.Add(FacilityType.Reception);
            if (d.Entrega == true) types.Add(FacilityType.Transfer);
            if (d.Acopio == true) types.Add(FacilityType.Storage);
            if (d.Disposicion == true) types.Add(FacilityType.Disposal);
            if (d.Tratamiento == true) types.Add(FacilityType.Processing);
            if (d.Almacenamiento == true) types.Add(FacilityType.Containment);

            return types;
        }
    }
}
