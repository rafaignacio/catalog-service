using CatalogService.Core.DomainEvents;
using CatalogService.Infrastructure.Configurations;
using MediatR;
using Microsoft.Extensions.Options;

namespace CatalogService.Infrastructure.EventHandlers;

public class ItemRemovedEventHandler : INotificationHandler<ItemRemoved>
{
    private readonly IOptions<QueueConfiguration> _queueConfig;

    public ItemRemovedEventHandler(IOptions<QueueConfiguration> queueConfiguration)
    {
        _queueConfig = queueConfiguration;
    }

    public Task Handle(ItemRemoved notification, CancellationToken cancellationToken)
    {
        var broker = new MessageBroker(_queueConfig.Value.ConnectionString);

        broker.SendMessage(Constants.ItemChangesQueue, "ItemDeleted", notification);

        return Task.CompletedTask;
    }
}
