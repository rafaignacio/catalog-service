namespace CatalogService.API.Tests;

[SetUpFixture]
public class SetupTests
{
    [OneTimeSetUp]
    public async Task Setup()
    {
        File.Delete("./catalog.db");
    }
}
