using System.Text.Json.Serialization;

namespace CatalogService.Core.Models;

public record CategoryModel(string Name, string? Image, string? Parent)
{
    public CategoryModel(long id, string name, string? image, string? parent) : this(name, image, parent)
    {
        Id = id;
    }

    [JsonPropertyOrder(0)]
    public long Id { get; init; }
}