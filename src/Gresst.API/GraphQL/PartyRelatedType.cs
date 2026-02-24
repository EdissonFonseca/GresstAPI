using HotChocolate.Types;
using Gresst.Application.DTOs;

namespace Gresst.API.GraphQL;

public class PartyRelatedType : ObjectType<PartyRelatedDto>
{
    protected override void Configure(IObjectTypeDescriptor<PartyRelatedDto> descriptor)
    {
        descriptor.Ignore(f => f.Location);
    }
}