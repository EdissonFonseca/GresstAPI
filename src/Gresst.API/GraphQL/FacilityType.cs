using HotChocolate.Types;
using Gresst.Application.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gresst.API.GraphQL;

public class FacilityType : ObjectType<FacilityDto>
{
    protected override void Configure(IObjectTypeDescriptor<FacilityDto> descriptor)
    {
        descriptor.Field(f => f.Id).Type<NonNullType<IdType>>();
        descriptor.Field(f => f.Name).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.Description).Type<StringType>();
        descriptor.Field(f => f.Type).Type<StringType>();
        descriptor.Field("wasteTypes").ResolveWith<Resolvers>(r => r.GetWasteTypes(default!, default!)).Type<ListType<ObjectType<WasteTypeDto>>>();
    }

    private class Resolvers
    {
        public async Task<IEnumerable<WasteTypeDto>> GetWasteTypes([Parent] FacilityDto facility, [Service] Gresst.Application.Services.Interfaces.IFacilityService facilityService)
        {
            var f = await facilityService.GetByIdAsync(facility.Id);
            if (f == null) return Enumerable.Empty<WasteTypeDto>();
            // If FacilityDto had WasteTypes, return them. Currently it doesn't, so return empty.
            return Enumerable.Empty<WasteTypeDto>();
        }
    }
}
