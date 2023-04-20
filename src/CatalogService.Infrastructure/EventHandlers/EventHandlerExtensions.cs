using CatalogService.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Infrastructure.EventHandlers;

public static class EventHandlerExtensions
{
    public static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IEventDispatcher, EventDispatcher>();
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<EventDispatcher>();
        });

        return services;
    }
}
