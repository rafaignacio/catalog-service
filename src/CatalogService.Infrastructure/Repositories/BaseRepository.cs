using Dapper;
using Microsoft.Data.Sqlite;

namespace CatalogService.Infrastructure.Repositories;

public abstract class BaseRepository
{
    public string ConnectionString { get; internal set; } = string.Empty;

    public async Task<int> ExecuteAsync(string commandText, object? model = null, CancellationToken token = default)
    {
        using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync(token);

        try
        {
            var commandDefinition = new CommandDefinition(
                commandText, model, cancellationToken: token);

            return await connection.ExecuteAsync(commandDefinition);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public async Task<T> ExecuteScalarAsync<T>(string commandText, object? model = null, CancellationToken token = default)
    {
        using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync(token);

        try
        {
            var commandDefinition = new CommandDefinition(
                commandText, model, cancellationToken: token);

            return await connection.ExecuteScalarAsync<T>(commandDefinition);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string commandText, object? model = null, CancellationToken token = default)
    {
        using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync(token);

        try
        {
            var commandDefinition = new CommandDefinition(
                commandText, model, cancellationToken: token);

            return await connection.QueryAsync<T>(commandDefinition);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
