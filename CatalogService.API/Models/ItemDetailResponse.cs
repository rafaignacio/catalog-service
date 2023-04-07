using CatalogService.Core.Models;
using System.Text.Json.Serialization;

namespace CatalogService.API.Models;

public record ItemDetailResponse : ItemModel
{
    [JsonPropertyOrder(99)]
    public object Links { get; set; }
    public ItemDetailResponse(ItemModel item) : this(item.Id, item) { }
    public ItemDetailResponse(long id, ItemModel item) : base(id, item.Name, item.Description, item.Image, item.Category, item.Price, item.Amount)
    {
        var location = $"/items/{id}";
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
