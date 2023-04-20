using CatalogService.Core.Interfaces;
using MediatR;

namespace CatalogService.Core.Entities;

public abstract class Entity
{
    private List<INotification>? _events;

    protected void AddDomainEvent(INotification @event)
    {
        _events = _events ?? new List<INotification>();
        _events.Add(@event);
    }

    protected void RemoveDomainEvent(INotification @event) =>
        _events?.Remove(@event);

    public async Task DispatchEvents(IEventDispatcher eventDispatcher)
    {
        if (_events == null || _events.Count == 0)
            return;

        foreach (var e in _events)
            await eventDispatcher.Dispatch(e);
    }
}
