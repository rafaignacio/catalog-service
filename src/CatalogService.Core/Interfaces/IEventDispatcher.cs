using MediatR;

namespace CatalogService.Core.Interfaces;

public interface IEventDispatcher
{
    Task Dispatch(INotification @event);
}
