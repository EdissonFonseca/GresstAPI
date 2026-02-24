using Gresst.Application.DTOs;
using Gresst.Application.Services.Interfaces;

namespace Gresst.API.GraphQL;

public class PartiesQuery
{
    public async Task<IEnumerable<PartyRelatedDto>> GetParties([Service] IPartyService partyService)
    {
        return await partyService.GetAllAsync();
    }

    public async Task<PartyRelatedDto?> GetPartyById([Service] IPartyService partyService, string id)
    {
        return await partyService.GetByIdAsync(id);
    }
}
