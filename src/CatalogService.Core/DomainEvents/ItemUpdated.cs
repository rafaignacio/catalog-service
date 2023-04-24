using MediatR;

namespace CatalogService.Core.DomainEvents;

public record ItemUpdated(long Id, string Name, string? Description, string? Image, string Category, double Price, long Amount) : INotification;
