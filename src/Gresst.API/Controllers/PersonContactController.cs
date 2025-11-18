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

    // Note: Contact endpoints for clients and providers have been moved to PersonController
    // Use GET /api/person/{personId}/contact, POST /api/person/{personId}/contact, etc. instead
}

