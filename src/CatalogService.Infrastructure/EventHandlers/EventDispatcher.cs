using CatalogService.Core.Interfaces;
using MediatR;

namespace CatalogService.Infrastructure.EventHandlers;

public class EventDispatcher : IEventDispatcher
{
    private readonly IMediator _mediator;

    public EventDispatcher(IMediator mediator) => 
        _mediator = mediator;

    public async Task Dispatch(INotification @event) => 
        await _mediator.Publish(@event);
}
