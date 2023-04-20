namespace CatalogService.Infrastructure.Configurations;

public class QueueConfiguration
{
    public const string Name = "QueueConfig";

    public string ConnectionString { get; set; } = string.Empty;
}
