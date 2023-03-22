namespace CatalogService.Core.Models;

public record ItemModel(string Name, string? Description, string? Image, string Category, double Price, long Amount);
