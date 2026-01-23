using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(typeof(IEnumerable<PersonContactDto>), 200)]
        public async Task<ActionResult<IEnumerable<PersonContactDto>>> Get(CancellationToken cancellationToken)
        {
            try
            {
                var clients = await _clientService.GetAllAsync(cancellationToken);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients");
                return StatusCode(500, "Error retrieving clients");
            }
        }
    }
}
