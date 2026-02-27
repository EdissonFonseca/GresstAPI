using Gresst.Application.Common;
using Gresst.Domain.RouteProcesses;
using MediatR;

namespace Gresst.Application.RouteProcesses.Commands.CompleteRouteStop;

public class CompleteRouteStopCommandHandler : IRequestHandler<CompleteRouteStopCommand, Result<RouteProcessDto>>
{
    private readonly IRouteProcessRepository _repository;
    private readonly IPublisher _publisher;

    public CompleteRouteStopCommandHandler(IRouteProcessRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<Result<RouteProcessDto>> Handle(CompleteRouteStopCommand request, CancellationToken ct)
    {
        var route = await _repository.GetByIdAsync(request.RouteProcessId, ct);
        if (route == null)
            return Result<RouteProcessDto>.Fail("Route process not found.");

        try
        {
            route.CompleteStop(request.StopId, request.Notes, request.WasteItemIds);
            await _repository.UpdateAsync(route, ct);

            foreach (var evt in route.DomainEvents)
                await _publisher.Publish(evt, ct);
            route.ClearDomainEvents();

            return Result<RouteProcessDto>.Success(RouteProcessMapping.ToDto(route));
        }
        catch (RouteProcessDomainException ex)
        {
            return Result<RouteProcessDto>.Fail(ex.Message);
        }
    }
}
