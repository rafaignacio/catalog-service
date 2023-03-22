namespace CatalogService.Infrastructure.Configurations;

public class CatalogDatabaseConfiguration
{
    public const string Name = "CatalogDatabase";
    public CatalogDatabaseConfiguration() { }

    public string ConnectionString { get; set; } = string.Empty;

}
