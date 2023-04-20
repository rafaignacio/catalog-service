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
}
