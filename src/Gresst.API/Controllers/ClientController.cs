using Asp.Versioning;
using Azure;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using System.Net;
using System.Resources;
using System.Text.Json.Serialization;

namespace Gresst.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(
            IClientService clientService,
            ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        /// <summary>
        /// Get all contacts for the account person
        /// </summary>
        [HttpGet("get")]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        public async Task<ActionResult<IEnumerable<ClientDto>>> Get(CancellationToken cancellationToken)
        {
            try
            {
                var clients = await _clientService.GetAllAsync(cancellationToken);
                foreach (var c in clients)
                {
                    // Id ya es el IdPersona (string de BD); no convertir con Guid
                    c.IdPersona = c.Id ?? string.Empty;
                    c.Nombre = c.Name ?? string.Empty;
                }
                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients");
                return StatusCode(500, "Error retrieving clients");
            }
        }

        [HttpGet("get/{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ClientDto), 200)]
        public async Task<ActionResult<ClientDto>> Get(string id, CancellationToken cancellationToken)
        {
            try
            {
                // id es string (IdPersona de BD); pasar tal cual al servicio
                var client = await _clientService.GetByIdAsync(id, cancellationToken);
                if (client != null)
                {
                    client.IdPersona = client.Id ?? string.Empty;
                    client.Nombre = client.Name ?? string.Empty;
                }

                return Ok(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients");
                return StatusCode(500, "Error retrieving clients");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        [HttpPost("post")]
        public async Task<ActionResult> Post([FromBody] ClientDto cliente)
        {
            return Created();
        }
    }
}
