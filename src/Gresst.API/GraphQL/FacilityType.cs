using Gresst.Application.DTOs;

namespace Gresst.API.GraphQL;

public class FacilityType : ObjectType<FacilityDto>
{
    protected override void Configure(IObjectTypeDescriptor<FacilityDto> descriptor)
    {
        descriptor.Ignore(f => f.Location);
    }
}
