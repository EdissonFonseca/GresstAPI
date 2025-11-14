using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    /// <summary>
    /// GET: Obtener todos los clientes
    /// </summary>
    /// <remarks>
    /// Devuelve todos los clientes activos de la cuenta actual.
    /// Los clientes son Personas con rol de Cliente.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll(CancellationToken cancellationToken)
    {
        var clients = await _clientService.GetAllAsync(cancellationToken);
        return Ok(clients);
    }

    /// <summary>
    /// GET: Obtener cliente por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClientDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ClientDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var client = await _clientService.GetByIdAsync(id, cancellationToken);
        
        if (client == null)
            return NotFound(new { message = "Client not found" });

        return Ok(client);
    }

    /// <summary>
    /// GET: Obtener cliente por número de documento
    /// </summary>
    [HttpGet("document/{documentNumber}")]
    [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetByDocumentNumber(string documentNumber, CancellationToken cancellationToken)
    {
        var clients = await _clientService.GetByDocumentNumberAsync(documentNumber, cancellationToken);
        return Ok(clients);
    }

    /// <summary>
    /// POST: Crear nuevo cliente
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     POST /api/client
    ///     {
    ///         "name": "Empresa Cliente S.A.S.",
    ///         "documentNumber": "900123456-1",
    ///         "email": "contacto@cliente.com",
    ///         "phone": "+57 300 1234567",
    ///         "address": "Calle 100 #50-20, Bogotá",
    ///         "isGenerator": true
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ClientDto>> Create([FromBody] CreateClientDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var client = await _clientService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    /// <summary>
    /// PUT: Actualizar cliente existente
    /// </summary>
    /// <remarks>
    /// Solo se actualizan los campos proporcionados (PATCH-like).
    /// 
    /// Ejemplo:
    /// 
    ///     PUT /api/client/{id}
    ///     {
    ///         "id": "guid-client",
    ///         "name": "Empresa Cliente Actualizada",
    ///         "email": "nuevo@cliente.com"
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ClientDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ClientDto>> Update(Guid id, [FromBody] UpdateClientDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var client = await _clientService.UpdateAsync(dto, cancellationToken);
        
        if (client == null)
            return NotFound(new { message = "Client not found" });

        return Ok(client);
    }

    /// <summary>
    /// DELETE: Eliminar cliente (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _clientService.DeleteAsync(id, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "Client not found" });

        return NoContent();
    }
}

