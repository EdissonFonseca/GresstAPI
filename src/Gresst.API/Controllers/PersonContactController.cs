using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

/// <summary>
/// Controller for managing PersonContacts
/// Handles contacts for Account Person, Clients, and Providers
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class PersonContactController : ControllerBase
{
    private readonly IPersonContactService _personContactService;
    private readonly ILogger<PersonContactController> _logger;

    public PersonContactController(
        IPersonContactService personContactService,
        ILogger<PersonContactController> logger)
    {
        _personContactService = personContactService;
        _logger = logger;
    }

    #region Account Person Contacts

    /// <summary>
    /// Get all contacts for the account person
    /// </summary>
    [HttpGet("account")]
    [ProducesResponseType(typeof(IEnumerable<PersonContactDto>), 200)]
    public async Task<ActionResult<IEnumerable<PersonContactDto>>> GetAccountPersonContacts(CancellationToken cancellationToken)
    {
        try
        {
            var contacts = await _personContactService.GetAccountPersonContactsAsync(cancellationToken);
            return Ok(contacts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account person contacts");
            return StatusCode(500, "Error retrieving contacts");
        }
    }

    /// <summary>
    /// Get a specific contact for the account person
    /// </summary>
    [HttpGet("account/{contactId}")]
    [ProducesResponseType(typeof(PersonContactDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonContactDto>> GetAccountPersonContact(
        Guid contactId,
        [FromQuery] string? relationshipType,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _personContactService.GetAccountPersonContactAsync(contactId, relationshipType, cancellationToken);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account person contact {ContactId}", contactId);
            return StatusCode(500, "Error retrieving contact");
        }
    }

    /// <summary>
    /// Create a new contact for the account person
    /// </summary>
    [HttpPost("account")]
    [ProducesResponseType(typeof(PersonContactDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PersonContactDto>> CreateAccountPersonContact(
        [FromBody] CreatePersonContactDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _personContactService.CreateAccountPersonContactAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetAccountPersonContact), new { contactId = contact.ContactId }, contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account person contact");
            return StatusCode(500, "Error creating contact");
        }
    }

    /// <summary>
    /// Update a contact for the account person
    /// </summary>
    [HttpPut("account")]
    [ProducesResponseType(typeof(PersonContactDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonContactDto>> UpdateAccountPersonContact(
        [FromBody] UpdatePersonContactDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _personContactService.UpdateAccountPersonContactAsync(dto, cancellationToken);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account person contact");
            return StatusCode(500, "Error updating contact");
        }
    }

    /// <summary>
    /// Delete a contact for the account person
    /// </summary>
    [HttpDelete("account/{contactId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteAccountPersonContact(
        Guid contactId,
        [FromQuery] string? relationshipType,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _personContactService.DeleteAccountPersonContactAsync(contactId, relationshipType, cancellationToken);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account person contact {ContactId}", contactId);
            return StatusCode(500, "Error deleting contact");
        }
    }

    #endregion

    #region Client Contacts

    /// <summary>
    /// Get all contacts for a client
    /// </summary>
    [HttpGet("client/{clientId}")]
    [ProducesResponseType(typeof(IEnumerable<PersonContactDto>), 200)]
    public async Task<ActionResult<IEnumerable<PersonContactDto>>> GetClientContacts(
        Guid clientId,
        CancellationToken cancellationToken)
    {
        try
        {
            var contacts = await _personContactService.GetClientContactsAsync(clientId, cancellationToken);
            return Ok(contacts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client contacts for {ClientId}", clientId);
            return StatusCode(500, "Error retrieving contacts");
        }
    }

    /// <summary>
    /// Get a specific contact for a client
    /// </summary>
    [HttpGet("client/{clientId}/contact/{contactId}")]
    [ProducesResponseType(typeof(PersonContactDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonContactDto>> GetClientContact(
        Guid clientId,
        Guid contactId,
        [FromQuery] string? relationshipType,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _personContactService.GetClientContactAsync(clientId, contactId, relationshipType, cancellationToken);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client contact {ContactId} for {ClientId}", contactId, clientId);
            return StatusCode(500, "Error retrieving contact");
        }
    }

    /// <summary>
    /// Create a new contact for a client
    /// </summary>
    [HttpPost("client/{clientId}")]
    [ProducesResponseType(typeof(PersonContactDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PersonContactDto>> CreateClientContact(
        Guid clientId,
        [FromBody] CreatePersonContactDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _personContactService.CreateClientContactAsync(clientId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetClientContact), new { clientId, contactId = contact.ContactId }, contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client contact for {ClientId}", clientId);
            return StatusCode(500, "Error creating contact");
        }
    }

    /// <summary>
    /// Update a contact for a client
    /// </summary>
    [HttpPut("client/{clientId}")]
    [ProducesResponseType(typeof(PersonContactDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonContactDto>> UpdateClientContact(
        Guid clientId,
        [FromBody] UpdatePersonContactDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _personContactService.UpdateClientContactAsync(clientId, dto, cancellationToken);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client contact for {ClientId}", clientId);
            return StatusCode(500, "Error updating contact");
        }
    }

    /// <summary>
    /// Delete a contact for a client
    /// </summary>
    [HttpDelete("client/{clientId}/contact/{contactId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteClientContact(
        Guid clientId,
        Guid contactId,
        [FromQuery] string? relationshipType,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _personContactService.DeleteClientContactAsync(clientId, contactId, relationshipType, cancellationToken);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client contact {ContactId} for {ClientId}", contactId, clientId);
            return StatusCode(500, "Error deleting contact");
        }
    }

    #endregion

    #region Provider Contacts

    /// <summary>
    /// Get all contacts for a provider
    /// </summary>
    [HttpGet("provider/{providerId}")]
    [ProducesResponseType(typeof(IEnumerable<PersonContactDto>), 200)]
    public async Task<ActionResult<IEnumerable<PersonContactDto>>> GetProviderContacts(
        Guid providerId,
        CancellationToken cancellationToken)
    {
        try
        {
            var contacts = await _personContactService.GetProviderContactsAsync(providerId, cancellationToken);
            return Ok(contacts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting provider contacts for {ProviderId}", providerId);
            return StatusCode(500, "Error retrieving contacts");
        }
    }

    /// <summary>
    /// Get a specific contact for a provider
    /// </summary>
    [HttpGet("provider/{providerId}/contact/{contactId}")]
    [ProducesResponseType(typeof(PersonContactDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonContactDto>> GetProviderContact(
        Guid providerId,
        Guid contactId,
        [FromQuery] string? relationshipType,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _personContactService.GetProviderContactAsync(providerId, contactId, relationshipType, cancellationToken);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting provider contact {ContactId} for {ProviderId}", contactId, providerId);
            return StatusCode(500, "Error retrieving contact");
        }
    }

    /// <summary>
    /// Create a new contact for a provider
    /// </summary>
    [HttpPost("provider/{providerId}")]
    [ProducesResponseType(typeof(PersonContactDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PersonContactDto>> CreateProviderContact(
        Guid providerId,
        [FromBody] CreatePersonContactDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _personContactService.CreateProviderContactAsync(providerId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetProviderContact), new { providerId, contactId = contact.ContactId }, contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating provider contact for {ProviderId}", providerId);
            return StatusCode(500, "Error creating contact");
        }
    }

    /// <summary>
    /// Update a contact for a provider
    /// </summary>
    [HttpPut("provider/{providerId}")]
    [ProducesResponseType(typeof(PersonContactDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonContactDto>> UpdateProviderContact(
        Guid providerId,
        [FromBody] UpdatePersonContactDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _personContactService.UpdateProviderContactAsync(providerId, dto, cancellationToken);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating provider contact for {ProviderId}", providerId);
            return StatusCode(500, "Error updating contact");
        }
    }

    /// <summary>
    /// Delete a contact for a provider
    /// </summary>
    [HttpDelete("provider/{providerId}/contact/{contactId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProviderContact(
        Guid providerId,
        Guid contactId,
        [FromQuery] string? relationshipType,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _personContactService.DeleteProviderContactAsync(providerId, contactId, relationshipType, cancellationToken);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting provider contact {ContactId} for {ProviderId}", contactId, providerId);
            return StatusCode(500, "Error deleting contact");
        }
    }

    #endregion
}

