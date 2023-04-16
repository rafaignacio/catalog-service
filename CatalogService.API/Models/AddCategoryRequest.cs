using CatalogService.Core.Models;

namespace CatalogService.API.Models;

public class AddCategoryRequest
{
    public string Name { get; set; }
    public string? Image { get; set; }
    public string? Parent { get; set; }

    public CategoryModel ToCategoryModel() =>
        new(Name, Image, Parent);
}
