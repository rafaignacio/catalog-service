using Dapper;
using Microsoft.Data.Sqlite;

namespace CatalogService.Infrastructure;

public static class CatalogDatabase
{
    public static async Task Initiate(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(CreateCategoryTable());
        await connection.ExecuteAsync(CreateItemTable());
        connection.Close();
    }

    private static CommandDefinition CreateCategoryTable() =>
        new(@"
            CREATE TABLE IF NOT EXISTS Category (
                Name    VARCHAR(50) PRIMARY KEY,
                Image   TEXT,
                Parent  VARCHAR(50),
                FOREIGN KEY (Parent) REFERENCES Category(Name))");

    private static CommandDefinition CreateItemTable() =>
        new(@"
            CREATE TABLE IF NOT EXISTS Item (
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
