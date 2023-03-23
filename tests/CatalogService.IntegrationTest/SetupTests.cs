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
        await _connection.ExecuteAsync(CreateCategoryTable());
        await _connection.ExecuteAsync(CreateItemTable());
    }

    [OneTimeTearDown]
    public async Task Clean()
    {
        await _connection.CloseAsync();
        _connection?.Dispose();
    }

    public CommandDefinition CreateCategoryTable() =>
        new (@"
            CREATE TABLE Category (
                Name    VARCHAR(50) PRIMARY KEY,
                Image   TEXT,
                Parent  VARCHAR(50),
                FOREIGN KEY (Parent) REFERENCES Category(Name))");

    public CommandDefinition CreateItemTable() =>
        new(@"
            CREATE TABLE Item (
                Id          INTEGER PRIMARY KEY,
                Name        VARCHAR(50) NOT NULL,
                Category    VARCHAR(50) NOT NULL,
                Description TEXT,
                Image       TEXT,
                Price       REAL NOT NULL,
                Amount      INTEGER NOT NULL,
                UNIQUE (Name, Category),
                FOREIGN KEY (Category) REFERENCES Category(Name))");

}
