namespace CatalogService.Core.Models;

public record ItemDetailsModel(string Name, string? Description, string? Image, string Category, double Price, long Amount);
