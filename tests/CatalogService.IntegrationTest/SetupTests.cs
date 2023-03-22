using Dapper;
using Microsoft.Data.Sqlite;

namespace CatalogService.IntegrationTest;

[SetUpFixture]
public class SetupTests
{
    private SqliteConnection _connection;
    [OneTimeSetUp]
    public async Task Setup()
    {
        _connection = new SqliteConnection(Constants.ConnectionString);
        await _connection.OpenAsync();
        await _connection.ExecuteAsync(CreateCategoryDatabase());
    }

    [OneTimeTearDown]
    public async Task Clean()
    {
        await _connection.CloseAsync();
        _connection?.Dispose();
    }

    public CommandDefinition CreateCategoryDatabase() =>
        new (@"
            CREATE TABLE Category (
                Name    VARCHAR(50) PRIMARY KEY,
                Image   TEXT,
                Parent  VARCHAR(50))");

}
