using CatalogService.Core.DomainEvents;
using CatalogService.Infrastructure.Configurations;
using MediatR;
using Microsoft.Extensions.Options;

namespace CatalogService.Infrastructure.EventHandlers;

public class ItemUpdatedEventHandler : INotificationHandler<ItemUpdated>
{
    private readonly IOptions<QueueConfiguration> _queueConfig;

    public ItemUpdatedEventHandler(IOptions<QueueConfiguration> queueConfiguration)
    {
        _queueConfig = queueConfiguration;
    }

    public Task Handle(ItemUpdated notification, CancellationToken cancellationToken)
    {
        var broker = new MessageBroker(_queueConfig.Value.ConnectionString);

        broker.SendMessage(Constants.ItemChangesQueue, "ItemUpdated", notification);

        return Task.CompletedTask;
    }
}
