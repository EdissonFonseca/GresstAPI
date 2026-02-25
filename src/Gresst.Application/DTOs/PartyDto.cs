using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gresst.Application.DTOs
{
    /// <summary>
    /// DTO for PersonContact relationship
    /// </summary>
    public class PartyDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public PersonType? PersonType { get; set; }
        public string? DocumentNumber { get; set; }
        public int? CheckDigit { get; set; }
        public DocumentType? DocumentType { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? SignatureUrl { get; set; }
        public Point? Location { get; set; }
        public string? LocalityId { get; set; }
        public required bool IsActive { get; set; }
        public List<FacilityDto> Facilities { get; set; } = new List<FacilityDto>();
    }

    public class CreatePartyDto
    {
        public required string Name { get; set; }
        public PersonType? PersonType { get; set; }
        public string? DocumentNumber { get; set; }
        public int? CheckDigit { get; set; }
        public DocumentType? DocumentType { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public Point? Location { get; set; }
        public string? SignatureUrl { get; set; }
        public string? LocalityId { get; set; }
        public required string RoleId { get; set; }
    }
    public class UpdatePartyDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public PersonType? PersonType { get; set; }
        public string? DocumentNumber { get; set; }
        public int? CheckDigit { get; set; }
        public DocumentType? DocumentType { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public Point? Location { get; set; }
        public string? SignatureUrl { get; set; }
        public string? LocalityId { get; set; }
        public required bool IsActive { get; set; }
        public required string RoleId { get; set; }
    }
}
