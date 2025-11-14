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
    Task<IEnumerable<PersonContact>> GetContactsByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all persons where a contact is associated
    /// </summary>
    Task<IEnumerable<PersonContact>> GetPersonsByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get a specific contact relationship
    /// </summary>
    Task<PersonContact?> GetByPersonAndContactIdAsync(Guid personId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get contacts by relationship type for a person
    /// </summary>
    Task<IEnumerable<PersonContact>> GetContactsByPersonAndRelationshipAsync(Guid personId, string relationshipType, CancellationToken cancellationToken = default);
}

