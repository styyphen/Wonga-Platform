using Npgsql;

namespace Wonga.Services.Identity.Infrastructure;

public sealed class IdentityDatabaseInitializer(NpgsqlDataSource dataSource)
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        const string sql = """
                           CREATE SCHEMA IF NOT EXISTS auth;

                           CREATE TABLE IF NOT EXISTS auth.users (
                               id UUID PRIMARY KEY,
                               email TEXT NOT NULL UNIQUE,
                               password_hash TEXT NOT NULL,
                               password_salt TEXT NOT NULL,
                               created_utc TIMESTAMPTZ NOT NULL
                           );
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
