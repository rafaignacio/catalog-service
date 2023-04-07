namespace CatalogService.API.Models;

public record UpdateItemModel(string Name, string? Description, string? Image, string Category, double Price, long Amount);
