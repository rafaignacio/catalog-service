using CatalogService.Core.Models;

namespace CatalogService.API.Models;

public class AddItemModel
{
    public AddItemModel() { }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string Category { get; set; } = string.Empty;
    public double Price { get; set; }
    public long Amount { get; set; }

    public ItemModel ToItemModel() =>
        new(Name, Description, Image, Category, Price, Amount);
}