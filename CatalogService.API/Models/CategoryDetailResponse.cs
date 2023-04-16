using CatalogService.Core.Entities;
using CatalogService.Core.Models;
using System.Text.Json.Serialization;

public record CategoryDetailResponse : CategoryModel
{
    [JsonPropertyOrder(99)]
    public object Links { get; set; }

    [JsonConstructor]
    public CategoryDetailResponse(long id, string name, string image, string parent, object links) : this(id, new CategoryModel(name, image, parent))
    {
        Links = links;
    }

    public CategoryDetailResponse(CategoryModel category) : this(category.Id, category) { }
    public CategoryDetailResponse(long id, CategoryModel category) : base(id, category.Name, category.Image, category.Parent)
    {
        var location = $"/categories/{id}";
        Links = new[]
            {
                new {
                    Rel = "update",
                    Verb = "PUT",
                    Href = location
                },
                new {
                    Rel = "delete",
                    Verb = "DELETE",
                    Href = location
                },
            };
    }
}