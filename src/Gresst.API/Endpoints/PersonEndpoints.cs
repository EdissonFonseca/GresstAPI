using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Endpoints;

public static class PersonEndpoints
{
    public static RouteGroupBuilder Map(this RouteGroupBuilder group)
    {
        var person = group.MapGroup("/people")
            .WithTags("Person");

        // Material
        person.MapGet("{personId}/material", async (string personId, IMaterialService materialService, CancellationToken ct) =>
            {
                var clientMaterials = await materialService.GetClientMaterialsAsync(personId, ct);
                if (clientMaterials.Any())
                    return Results.Ok(clientMaterials);
                var providerMaterials = await materialService.GetProviderMaterialsAsync(personId, ct);
                return Results.Ok(providerMaterials);
            }).WithName("GetPersonMaterials");

        person.MapPost("{personId}/material", async (string personId, [FromBody] CreateMaterialDto dto, IMaterialService materialService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                try
                {
                    var material = await materialService.CreateClientMaterialAsync(personId, dto, ct);
                    return Results.Created($"/api/v1/people/{personId}/material", material);
                }
                catch
                {
                    var material = await materialService.CreateProviderMaterialAsync(personId, dto, ct);
                    return Results.Created($"/api/v1/people/{personId}/material", material);
                }
            }).WithName("CreatePersonMaterial");

        // Service
        person.MapGet("{personId}/service", async (string personId, IServiceService serviceService, CancellationToken ct) =>
            {
                var list = await serviceService.GetProviderServicesAsync(personId, ct);
                return Results.Ok(list);
            }).WithName("GetPersonServices");

        person.MapPost("{personId}/service", async (string personId, [FromBody] CreatePersonServiceDto dto, IServiceService serviceService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                var personService = await serviceService.CreateProviderServiceAsync(personId, dto, ct);
                return Results.Created($"/api/v1/people/{personId}/service", personService);
            }).WithName("CreatePersonService");

        person.MapPut("{personId}/service", async (string personId, [FromBody] UpdatePersonServiceDto dto, IServiceService serviceService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                var personService = await serviceService.UpdateProviderServiceAsync(personId, dto, ct);
                if (personService == null) return Results.NotFound(new { message = "PersonService not found" });
                return Results.Ok(personService);
            }).WithName("UpdatePersonService");

        person.MapDelete("{personId}/service/{serviceId}/{startDate:datetime}", async (string personId, string serviceId, DateTime startDate, IServiceService serviceService, CancellationToken ct) =>
            {
                var success = await serviceService.DeleteProviderServiceAsync(personId, serviceId, startDate, ct);
                if (!success) return Results.NotFound(new { message = "PersonService not found" });
                return Results.NoContent();
            }).WithName("DeletePersonService");

        // Treatment
        person.MapGet("{personId}/treatment", async (string personId, ITreatmentService treatmentService, CancellationToken ct) =>
            {
                var list = await treatmentService.GetProviderTreatmentsAsync(personId, ct);
                return Results.Ok(list);
            }).WithName("GetPersonTreatments");

        person.MapPost("{personId}/treatment", async (string personId, [FromBody] CreatePersonTreatmentDto dto, ITreatmentService treatmentService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                var personTreatment = await treatmentService.CreateProviderTreatmentAsync(personId, dto, ct);
                return Results.Created($"/api/v1/people/{personId}/treatment", personTreatment);
            }).WithName("CreatePersonTreatment");

        person.MapPut("{personId}/treatment", async (string personId, [FromBody] UpdatePersonTreatmentDto dto, ITreatmentService treatmentService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                var personTreatment = await treatmentService.UpdateProviderTreatmentAsync(personId, dto, ct);
                if (personTreatment == null) return Results.NotFound(new { message = "PersonTreatment not found" });
                return Results.Ok(personTreatment);
            }).WithName("UpdatePersonTreatment");

        person.MapDelete("{personId}/treatment/{treatmentId}", async (string personId, string treatmentId, ITreatmentService treatmentService, CancellationToken ct) =>
            {
                var success = await treatmentService.DeleteProviderTreatmentAsync(personId, treatmentId, ct);
                if (!success) return Results.NotFound(new { message = "PersonTreatment not found" });
                return Results.NoContent();
            }).WithName("DeletePersonTreatment");

        // WasteClass
        person.MapGet("{personId}/wasteclass", async (string personId, IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                var list = await wasteClassService.GetProviderWasteClassesAsync(personId, ct);
                return Results.Ok(list);
            }).WithName("GetPersonWasteClasses");

        person.MapPost("{personId}/wasteclass", async (string personId, [FromBody] CreatePersonWasteClassDto dto, IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                var personWasteClass = await wasteClassService.CreateProviderWasteClassAsync(personId, dto, ct);
                return Results.Created($"/api/v1/people/{personId}/wasteclass", personWasteClass);
            }).WithName("CreatePersonWasteClass");

        person.MapPut("{personId}/wasteclass", async (string personId, [FromBody] UpdatePersonWasteClassDto dto, IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                var personWasteClass = await wasteClassService.UpdateProviderWasteClassAsync(personId, dto, ct);
                if (personWasteClass == null) return Results.NotFound(new { message = "PersonWasteClass not found" });
                return Results.Ok(personWasteClass);
            }).WithName("UpdatePersonWasteClass");

        person.MapDelete("{personId}/wasteclass/{wasteClassId}", async (string personId, string wasteClassId, IWasteClassService wasteClassService, CancellationToken ct) =>
            {
                var success = await wasteClassService.DeleteProviderWasteClassAsync(personId, wasteClassId, ct);
                if (!success) return Results.NotFound(new { message = "PersonWasteClass not found" });
                return Results.NoContent();
            }).WithName("DeletePersonWasteClass");

        // Facility
        person.MapGet("{personId}/facility", async (string personId, IFacilityService facilityService, CancellationToken ct) =>
            {
                var clientFacilities = await facilityService.GetClientFacilitiesAsync(personId, ct);
                if (clientFacilities.Any()) return Results.Ok(clientFacilities);
                var providerFacilities = await facilityService.GetProviderFacilitiesAsync(personId, ct);
                return Results.Ok(providerFacilities);
            }).WithName("GetPersonFacilities");

        person.MapPost("{personId}/facility", async (string personId, [FromBody] CreateFacilityDto dto, IFacilityService facilityService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                try
                {
                    var facility = await facilityService.CreateClientFacilityAsync(personId, dto, ct);
                    return Results.Created($"/api/v1/people/{personId}/facility", facility);
                }
                catch
                {
                    var facility = await facilityService.CreateProviderFacilityAsync(personId, dto, ct);
                    return Results.Created($"/api/v1/people/{personId}/facility", facility);
                }
            }).WithName("CreatePersonFacility");

        person.MapGet("{personId}/facility/{facilityId}", async (string personId, string facilityId, IFacilityService facilityService, CancellationToken ct) =>
            {
                var clientFacilities = await facilityService.GetClientFacilitiesAsync(personId, ct);
                var facility = clientFacilities.FirstOrDefault(f => f.Id == facilityId);
                if (facility != null) return Results.Ok(facility);
                var providerFacilities = await facilityService.GetProviderFacilitiesAsync(personId, ct);
                facility = providerFacilities.FirstOrDefault(f => f.Id == facilityId);
                if (facility == null) return Results.NotFound(new { message = "Facility not found or does not belong to this person" });
                return Results.Ok(facility);
            }).WithName("GetPersonFacility");

        person.MapGet("{personId}/facility/{facilityId}/material", async (string personId, string facilityId, IFacilityService facilityService, IMaterialService materialService, CancellationToken ct) =>
            {
                var clientFacilities = await facilityService.GetClientFacilitiesAsync(personId, ct);
                var facility = clientFacilities.FirstOrDefault(f => f.Id == facilityId) ?? (await facilityService.GetProviderFacilitiesAsync(personId, ct)).FirstOrDefault(f => f.Id == facilityId);
                if (facility == null) return Results.NotFound(new { message = "Facility not found or does not belong to this person" });
                var materials = await materialService.GetFacilityMaterialsAsync(facilityId, ct);
                return Results.Ok(materials);
            }).WithName("GetPersonFacilityMaterials");

        person.MapPost("{personId}/facility/{facilityId}/material", async (string personId, string facilityId, [FromBody] CreateMaterialDto dto, IFacilityService facilityService, IMaterialService materialService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                var clientFacilities = await facilityService.GetClientFacilitiesAsync(personId, ct);
                var facility = clientFacilities.FirstOrDefault(f => f.Id == facilityId) ?? (await facilityService.GetProviderFacilitiesAsync(personId, ct)).FirstOrDefault(f => f.Id == facilityId);
                if (facility == null) return Results.NotFound(new { message = "Facility not found or does not belong to this person" });
                var material = await materialService.CreateFacilityMaterialAsync(facilityId, dto, ct);
                return Results.Created($"/api/v1/people/{personId}/facility/{facilityId}/material", material);
            }).WithName("CreatePersonFacilityMaterial");

        // Vehicle
        person.MapGet("{personId}/vehicle", async (string personId, IVehicleService vehicleService, CancellationToken ct) =>
            {
                var clientVehicles = await vehicleService.GetClientVehiclesAsync(personId, ct);
                if (clientVehicles.Any()) return Results.Ok(clientVehicles);
                var providerVehicles = await vehicleService.GetProviderVehiclesAsync(personId, ct);
                return Results.Ok(providerVehicles);
            }).WithName("GetPersonVehicles");

        person.MapPost("{personId}/vehicle", async (string personId, [FromBody] CreateVehicleDto dto, IVehicleService vehicleService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                try
                {
                    var vehicle = await vehicleService.CreateClientVehicleAsync(personId, dto, ct);
                    return Results.Created($"/api/v1/people/{personId}/vehicle", vehicle);
                }
                catch
                {
                    var vehicle = await vehicleService.CreateProviderVehicleAsync(personId, dto, ct);
                    return Results.Created($"/api/v1/people/{personId}/vehicle", vehicle);
                }
            }).WithName("CreatePersonVehicle");

        // Contact
        person.MapGet("{personId}/contact", async (string personId, IPersonContactService personContactService, CancellationToken ct) =>
            {
                var list = await personContactService.GetPersonContactsAsync(personId, ct);
                return Results.Ok(list);
            }).WithName("GetPersonContacts");

        person.MapGet("{personId}/contact/{contactId}", async (string personId, string contactId, [FromQuery] string? relationshipType, IPersonContactService personContactService, CancellationToken ct) =>
            {
                var contact = await personContactService.GetPersonContactAsync(personId, contactId, relationshipType, ct);
                if (contact == null) return Results.NotFound(new { message = "Contact not found or does not belong to this person" });
                return Results.Ok(contact);
            }).WithName("GetPersonContact");

        person.MapPost("{personId}/contact", async (string personId, [FromBody] CreatePersonContactDto dto, IPersonContactService personContactService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                var contact = await personContactService.CreatePersonContactAsync(personId, dto, ct);
                return Results.Created($"/api/v1/people/{personId}/contact", contact);
            }).WithName("CreatePersonContact");

        person.MapPut("{personId}/contact", async (string personId, [FromBody] UpdatePersonContactDto dto, IPersonContactService personContactService, CancellationToken ct) =>
            {
                if (dto == null) return Results.BadRequest();
                var contact = await personContactService.UpdatePersonContactAsync(personId, dto, ct);
                if (contact == null) return Results.NotFound(new { message = "Contact not found or does not belong to this person" });
                return Results.Ok(contact);
            }).WithName("UpdatePersonContact");

        person.MapDelete("{personId}/contact/{contactId}", async (string personId, string contactId, [FromQuery] string? relationshipType, IPersonContactService personContactService, CancellationToken ct) =>
            {
                var deleted = await personContactService.DeletePersonContactAsync(personId, contactId, relationshipType, ct);
                if (!deleted) return Results.NotFound(new { message = "Contact not found or does not belong to this person" });
                return Results.NoContent();
            }).WithName("DeletePersonContact");

        return group;
    }
}
