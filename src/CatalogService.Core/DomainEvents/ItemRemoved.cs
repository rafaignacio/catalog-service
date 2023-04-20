using MediatR;

namespace CatalogService.Core.DomainEvents;

public record ItemRemoved(long Id) : INotification;
