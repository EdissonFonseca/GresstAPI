using Gresst.Application.Common;
using Gresst.Domain.RouteProcesses;
using MediatR;

namespace Gresst.Application.RouteProcesses.Commands.CreateRouteProcess;

public class CreateRouteProcessCommandHandler : IRequestHandler<CreateRouteProcessCommand, Result<RouteProcessDto>>
{
    private readonly IRouteProcessRepository _repository;
    private readonly IPublisher _publisher;

    public CreateRouteProcessCommandHandler(IRouteProcessRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<Result<RouteProcessDto>> Handle(CreateRouteProcessCommand request, CancellationToken ct)
    {
        try
        {
            var stops = request.Stops
                .Select(s => (s.LocationId, s.OperationType, s.ResponsiblePartyId))
                .ToList();

            var route = RouteProcess.Create(request.VehicleId, request.DriverId, stops);

            await _repository.AddAsync(route, ct);

            foreach (var evt in route.DomainEvents)
                await _publisher.Publish(evt, ct);
            route.ClearDomainEvents();

            var dto = RouteProcessMapping.ToDto(route);
            return Result<RouteProcessDto>.Success(dto);
        }
        catch (RouteProcessDomainException ex)
        {
            return Result<RouteProcessDto>.Fail(ex.Message);
        }
    }
}
