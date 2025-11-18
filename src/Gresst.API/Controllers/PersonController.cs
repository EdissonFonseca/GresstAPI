using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PersonController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly IProviderService _providerService;
    private readonly IMaterialService _materialService;
    private readonly IServiceService _serviceService;
    private readonly ITreatmentService _treatmentService;
    private readonly IWasteClassService _wasteClassService;
    private readonly IFacilityService _facilityService;
    private readonly IVehicleService _vehicleService;
    private readonly IPersonContactService _personContactService;

    public PersonController(
        IClientService clientService,
        IProviderService providerService,
        IMaterialService materialService,
        IServiceService serviceService,
        ITreatmentService treatmentService,
        IWasteClassService wasteClassService,
        IFacilityService facilityService,
        IVehicleService vehicleService,
        IPersonContactService personContactService)
    {
        _clientService = clientService;
        _providerService = providerService;
        _materialService = materialService;
        _serviceService = serviceService;
        _treatmentService = treatmentService;
        _wasteClassService = wasteClassService;
        _facilityService = facilityService;
        _vehicleService = vehicleService;
        _personContactService = personContactService;
    }

    // Material endpoints - Nested under person
    /// <summary>
    /// GET: Obtener materiales de una persona (cliente o proveedor)
    /// </summary>
    [HttpGet("{personId}/material")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), 200)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetPersonMaterials(Guid personId, CancellationToken cancellationToken)
    {
        // Try as client first, then as provider
        var clientMaterials = await _materialService.GetClientMaterialsAsync(personId, cancellationToken);
        if (clientMaterials.Any())
            return Ok(clientMaterials);

        var providerMaterials = await _materialService.GetProviderMaterialsAsync(personId, cancellationToken);
        return Ok(providerMaterials);
    }

    /// <summary>
    /// POST: Crear material para una persona (cliente o proveedor)
    /// </summary>
    [HttpPost("{personId}/material")]
    [ProducesResponseType(typeof(MaterialDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<MaterialDto>> CreatePersonMaterial(Guid personId, [FromBody] CreateMaterialDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Try as client first, then as provider
        try
        {
            var material = await _materialService.CreateClientMaterialAsync(personId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetPersonMaterials), new { personId }, material);
        }
        catch
        {
            // If fails as client, try as provider
            var material = await _materialService.CreateProviderMaterialAsync(personId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetPersonMaterials), new { personId }, material);
        }
    }

    // Service endpoints - Nested under person
    /// <summary>
    /// GET: Obtener servicios de una persona (proveedor)
    /// </summary>
    [HttpGet("{personId}/service")]
    [ProducesResponseType(typeof(IEnumerable<PersonServiceDto>), 200)]
    public async Task<ActionResult<IEnumerable<PersonServiceDto>>> GetPersonServices(Guid personId, CancellationToken cancellationToken)
    {
        var services = await _serviceService.GetProviderServicesAsync(personId, cancellationToken);
        return Ok(services);
    }

    /// <summary>
    /// POST: Crear servicio para una persona (proveedor)
    /// </summary>
    [HttpPost("{personId}/service")]
    [ProducesResponseType(typeof(PersonServiceDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PersonServiceDto>> CreatePersonService(Guid personId, [FromBody] CreatePersonServiceDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personService = await _serviceService.CreateProviderServiceAsync(personId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetPersonServices), new { personId }, personService);
    }

    /// <summary>
    /// PUT: Actualizar servicio de una persona (proveedor)
    /// </summary>
    [HttpPut("{personId}/service")]
    [ProducesResponseType(typeof(PersonServiceDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonServiceDto>> UpdatePersonService(Guid personId, [FromBody] UpdatePersonServiceDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personService = await _serviceService.UpdateProviderServiceAsync(personId, dto, cancellationToken);
        
        if (personService == null)
            return NotFound(new { message = "PersonService not found" });

        return Ok(personService);
    }

    /// <summary>
    /// DELETE: Eliminar servicio de una persona (proveedor)
    /// </summary>
    [HttpDelete("{personId}/service/{serviceId}/{startDate}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeletePersonService(Guid personId, Guid serviceId, DateTime startDate, CancellationToken cancellationToken)
    {
        var success = await _serviceService.DeleteProviderServiceAsync(personId, serviceId, startDate, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "PersonService not found" });

        return NoContent();
    }

    // Treatment endpoints - Nested under person
    /// <summary>
    /// GET: Obtener tratamientos de una persona (proveedor)
    /// </summary>
    [HttpGet("{personId}/treatment")]
    [ProducesResponseType(typeof(IEnumerable<PersonTreatmentDto>), 200)]
    public async Task<ActionResult<IEnumerable<PersonTreatmentDto>>> GetPersonTreatments(Guid personId, CancellationToken cancellationToken)
    {
        var treatments = await _treatmentService.GetProviderTreatmentsAsync(personId, cancellationToken);
        return Ok(treatments);
    }

    /// <summary>
    /// POST: Crear tratamiento para una persona (proveedor)
    /// </summary>
    [HttpPost("{personId}/treatment")]
    [ProducesResponseType(typeof(PersonTreatmentDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PersonTreatmentDto>> CreatePersonTreatment(Guid personId, [FromBody] CreatePersonTreatmentDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personTreatment = await _treatmentService.CreateProviderTreatmentAsync(personId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetPersonTreatments), new { personId }, personTreatment);
    }

    /// <summary>
    /// PUT: Actualizar tratamiento de una persona (proveedor)
    /// </summary>
    [HttpPut("{personId}/treatment")]
    [ProducesResponseType(typeof(PersonTreatmentDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonTreatmentDto>> UpdatePersonTreatment(Guid personId, [FromBody] UpdatePersonTreatmentDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personTreatment = await _treatmentService.UpdateProviderTreatmentAsync(personId, dto, cancellationToken);
        
        if (personTreatment == null)
            return NotFound(new { message = "PersonTreatment not found" });

        return Ok(personTreatment);
    }

    /// <summary>
    /// DELETE: Eliminar tratamiento de una persona (proveedor)
    /// </summary>
    [HttpDelete("{personId}/treatment/{treatmentId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeletePersonTreatment(Guid personId, Guid treatmentId, CancellationToken cancellationToken)
    {
        var success = await _treatmentService.DeleteProviderTreatmentAsync(personId, treatmentId, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "PersonTreatment not found" });

        return NoContent();
    }

    // WasteClass endpoints - Nested under person
    /// <summary>
    /// GET: Obtener clases de residuo de una persona (proveedor)
    /// </summary>
    [HttpGet("{personId}/wasteclass")]
    [ProducesResponseType(typeof(IEnumerable<PersonWasteClassDto>), 200)]
    public async Task<ActionResult<IEnumerable<PersonWasteClassDto>>> GetPersonWasteClasses(Guid personId, CancellationToken cancellationToken)
    {
        var wasteClasses = await _wasteClassService.GetProviderWasteClassesAsync(personId, cancellationToken);
        return Ok(wasteClasses);
    }

    /// <summary>
    /// POST: Crear clase de residuo para una persona (proveedor)
    /// </summary>
    [HttpPost("{personId}/wasteclass")]
    [ProducesResponseType(typeof(PersonWasteClassDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PersonWasteClassDto>> CreatePersonWasteClass(Guid personId, [FromBody] CreatePersonWasteClassDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personWasteClass = await _wasteClassService.CreateProviderWasteClassAsync(personId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetPersonWasteClasses), new { personId }, personWasteClass);
    }

    /// <summary>
    /// PUT: Actualizar clase de residuo de una persona (proveedor)
    /// </summary>
    [HttpPut("{personId}/wasteclass")]
    [ProducesResponseType(typeof(PersonWasteClassDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonWasteClassDto>> UpdatePersonWasteClass(Guid personId, [FromBody] UpdatePersonWasteClassDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personWasteClass = await _wasteClassService.UpdateProviderWasteClassAsync(personId, dto, cancellationToken);
        
        if (personWasteClass == null)
            return NotFound(new { message = "PersonWasteClass not found" });

        return Ok(personWasteClass);
    }

    /// <summary>
    /// DELETE: Eliminar clase de residuo de una persona (proveedor)
    /// </summary>
    [HttpDelete("{personId}/wasteclass/{wasteClassId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeletePersonWasteClass(Guid personId, Guid wasteClassId, CancellationToken cancellationToken)
    {
        var success = await _wasteClassService.DeleteProviderWasteClassAsync(personId, wasteClassId, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "PersonWasteClass not found" });

        return NoContent();
    }

    // Facility endpoints - Nested under person
    /// <summary>
    /// GET: Obtener facilities de una persona (cliente o proveedor)
    /// </summary>
    [HttpGet("{personId}/facility")]
    [ProducesResponseType(typeof(IEnumerable<FacilityDto>), 200)]
    public async Task<ActionResult<IEnumerable<FacilityDto>>> GetPersonFacilities(Guid personId, CancellationToken cancellationToken)
    {
        // Try as client first, then as provider
        var clientFacilities = await _facilityService.GetClientFacilitiesAsync(personId, cancellationToken);
        if (clientFacilities.Any())
            return Ok(clientFacilities);

        var providerFacilities = await _facilityService.GetProviderFacilitiesAsync(personId, cancellationToken);
        return Ok(providerFacilities);
    }

    /// <summary>
    /// POST: Crear facility para una persona (cliente o proveedor)
    /// </summary>
    [HttpPost("{personId}/facility")]
    [ProducesResponseType(typeof(FacilityDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<FacilityDto>> CreatePersonFacility(Guid personId, [FromBody] CreateFacilityDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Try as client first, then as provider
        try
        {
            var facility = await _facilityService.CreateClientFacilityAsync(personId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetPersonFacilities), new { personId }, facility);
        }
        catch
        {
            // If fails as client, try as provider
            var facility = await _facilityService.CreateProviderFacilityAsync(personId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetPersonFacilities), new { personId }, facility);
        }
    }

    /// <summary>
    /// GET: Obtener un facility específico de una persona
    /// </summary>
    [HttpGet("{personId}/facility/{facilityId}")]
    [ProducesResponseType(typeof(FacilityDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<FacilityDto>> GetPersonFacility(Guid personId, Guid facilityId, CancellationToken cancellationToken)
    {
        // Verify the facility belongs to the person
        var facilities = await GetPersonFacilities(personId, cancellationToken);
        var facility = facilities.Value?.FirstOrDefault(f => f.Id == facilityId);
        
        if (facility == null)
            return NotFound(new { message = "Facility not found or does not belong to this person" });

        return Ok(facility);
    }

    /// <summary>
    /// GET: Obtener materiales de un facility específico de una persona
    /// </summary>
    [HttpGet("{personId}/facility/{facilityId}/material")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetPersonFacilityMaterials(Guid personId, Guid facilityId, CancellationToken cancellationToken)
    {
        // First verify the facility belongs to the person
        var facilities = await GetPersonFacilities(personId, cancellationToken);
        var facility = facilities.Value?.FirstOrDefault(f => f.Id == facilityId);
        
        if (facility == null)
            return NotFound(new { message = "Facility not found or does not belong to this person" });

        // Get materials for the facility
        var materials = await _materialService.GetFacilityMaterialsAsync(facilityId, cancellationToken);
        return Ok(materials);
    }

    /// <summary>
    /// POST: Crear material para un facility específico de una persona
    /// </summary>
    [HttpPost("{personId}/facility/{facilityId}/material")]
    [ProducesResponseType(typeof(MaterialDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<MaterialDto>> CreatePersonFacilityMaterial(Guid personId, Guid facilityId, [FromBody] CreateMaterialDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // First verify the facility belongs to the person
        var facilities = await GetPersonFacilities(personId, cancellationToken);
        var facility = facilities.Value?.FirstOrDefault(f => f.Id == facilityId);
        
        if (facility == null)
            return NotFound(new { message = "Facility not found or does not belong to this person" });

        // Create material for the facility
        var material = await _materialService.CreateFacilityMaterialAsync(facilityId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetPersonFacilityMaterials), new { personId, facilityId }, material);
    }

    // Vehicle endpoints - Nested under person
    /// <summary>
    /// GET: Obtener vehículos de una persona (cliente o proveedor)
    /// </summary>
    [HttpGet("{personId}/vehicle")]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), 200)]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetPersonVehicles(Guid personId, CancellationToken cancellationToken)
    {
        // Try as client first, then as provider
        var clientVehicles = await _vehicleService.GetClientVehiclesAsync(personId, cancellationToken);
        if (clientVehicles.Any())
            return Ok(clientVehicles);

        var providerVehicles = await _vehicleService.GetProviderVehiclesAsync(personId, cancellationToken);
        return Ok(providerVehicles);
    }

    /// <summary>
    /// POST: Crear vehículo para una persona (cliente o proveedor)
    /// </summary>
    [HttpPost("{personId}/vehicle")]
    [ProducesResponseType(typeof(VehicleDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<VehicleDto>> CreatePersonVehicle(Guid personId, [FromBody] CreateVehicleDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Try as client first, then as provider
        try
        {
            var vehicle = await _vehicleService.CreateClientVehicleAsync(personId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetPersonVehicles), new { personId }, vehicle);
        }
        catch
        {
            // If fails as client, try as provider
            var vehicle = await _vehicleService.CreateProviderVehicleAsync(personId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetPersonVehicles), new { personId }, vehicle);
        }
    }

    // Contact endpoints - Nested under person
    /// <summary>
    /// GET: Obtener contactos de una persona (cliente o proveedor)
    /// </summary>
    [HttpGet("{personId}/contact")]
    [ProducesResponseType(typeof(IEnumerable<PersonContactDto>), 200)]
    public async Task<ActionResult<IEnumerable<PersonContactDto>>> GetPersonContacts(Guid personId, CancellationToken cancellationToken)
    {
        var contacts = await _personContactService.GetPersonContactsAsync(personId, cancellationToken);
        return Ok(contacts);
    }

    /// <summary>
    /// GET: Obtener un contacto específico de una persona
    /// </summary>
    [HttpGet("{personId}/contact/{contactId}")]
    [ProducesResponseType(typeof(PersonContactDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonContactDto>> GetPersonContact(
        Guid personId,
        Guid contactId,
        [FromQuery] string? relationshipType,
        CancellationToken cancellationToken)
    {
        var contact = await _personContactService.GetPersonContactAsync(personId, contactId, relationshipType, cancellationToken);
        if (contact == null)
            return NotFound(new { message = "Contact not found or does not belong to this person" });

        return Ok(contact);
    }

    /// <summary>
    /// POST: Crear contacto para una persona (cliente o proveedor)
    /// </summary>
    [HttpPost("{personId}/contact")]
    [ProducesResponseType(typeof(PersonContactDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PersonContactDto>> CreatePersonContact(Guid personId, [FromBody] CreatePersonContactDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var contact = await _personContactService.CreatePersonContactAsync(personId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetPersonContact), new { personId, contactId = contact.ContactId }, contact);
    }

    /// <summary>
    /// PUT: Actualizar contacto de una persona (cliente o proveedor)
    /// </summary>
    [HttpPut("{personId}/contact")]
    [ProducesResponseType(typeof(PersonContactDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonContactDto>> UpdatePersonContact(Guid personId, [FromBody] UpdatePersonContactDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var contact = await _personContactService.UpdatePersonContactAsync(personId, dto, cancellationToken);
        if (contact == null)
            return NotFound(new { message = "Contact not found or does not belong to this person" });

        return Ok(contact);
    }

    /// <summary>
    /// DELETE: Eliminar contacto de una persona (cliente o proveedor)
    /// </summary>
    [HttpDelete("{personId}/contact/{contactId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeletePersonContact(
        Guid personId,
        Guid contactId,
        [FromQuery] string? relationshipType,
        CancellationToken cancellationToken)
    {
        var deleted = await _personContactService.DeletePersonContactAsync(personId, contactId, relationshipType, cancellationToken);
        if (!deleted)
            return NotFound(new { message = "Contact not found or does not belong to this person" });

        return NoContent();
    }
}

