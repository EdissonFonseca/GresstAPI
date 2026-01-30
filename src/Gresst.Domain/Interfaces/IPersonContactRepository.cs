using Gresst.Domain.Entities;

namespace Gresst.Domain.Interfaces;

/// <summary>
/// Repository interface for PersonContact with specific query methods
/// </summary>
public interface IPersonContactRepository : IRepository<PersonContact>
{
    /// <summary>
    /// Get all contacts for a specific person
    /// </summary>
    Task<IEnumerable<PersonContact>> GetContactsByPersonIdAsync(string personId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all persons where a contact is associated
    /// </summary>
    Task<IEnumerable<PersonContact>> GetPersonsByContactIdAsync(string contactId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get a specific contact relationship
    /// </summary>
    Task<PersonContact?> GetByPersonAndContactIdAsync(string personId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get contacts by relationship type for a person
    /// </summary>
    Task<IEnumerable<PersonContact>> GetContactsByPersonAndRelationshipAsync(string personId, string relationshipType, CancellationToken cancellationToken = default);
}

