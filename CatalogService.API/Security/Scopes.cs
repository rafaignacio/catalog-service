namespace CatalogService.API.Security;

public static class Scopes
{
    public const string CreateCategory = "catalog:categories:create";
    public const string ReadCategory = "catalog:categories:read";
    public const string UpdateCategory = "catalog:categories:update";
    public const string DeleteCategory = "catalog:categories:delete";
    public const string CreateItem = "catalog:items:create";
    public const string ReadItem = "catalog:items:read";
    public const string UpdateItem = "catalog:items:update";
    public const string DeleteItem = "catalog:items:delete";
}
