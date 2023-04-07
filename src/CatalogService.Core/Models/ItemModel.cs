namespace CatalogService.Core.Models;

public record ItemModel(string Name, string? Description, string? Image, string Category, double Price, long Amount)
{

    public ItemModel(long Id, string Name, string? Description, string? Image, string Category, double Price, long Amount) : this(Name, Description, Image, Category, Price, Amount)
    {
        this.Id = Id;
    }

    public long Id { get; init; }
}
