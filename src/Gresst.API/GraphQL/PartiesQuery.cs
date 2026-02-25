using Gresst.Application.DTOs;
using Gresst.Application.Services.Interfaces;
using HotChocolate.Authorization;

namespace Gresst.API.GraphQL;

[Authorize]
public class PartiesQuery
{
    public async Task<IEnumerable<PartyRelatedDto>> GetParties([Service] IPartyService partyService)
    {
        return await partyService.GetAllWithDetailsAsync();
    }

    public async Task<PartyRelatedDto?> GetPartyById([Service] IPartyService partyService, string id)
    {
        return await partyService.GetByIdAsync(id);
    }
}
