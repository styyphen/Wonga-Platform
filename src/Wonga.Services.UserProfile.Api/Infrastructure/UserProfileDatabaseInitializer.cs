using Npgsql;

namespace Wonga.Services.UserProfile.Infrastructure;

public sealed class UserProfileDatabaseInitializer(NpgsqlDataSource dataSource)
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        const string sql = """
                           CREATE SCHEMA IF NOT EXISTS profile;

                           CREATE TABLE IF NOT EXISTS profile.user_profiles (
                               user_id UUID PRIMARY KEY,
                               first_name TEXT NOT NULL,
                               last_name TEXT NOT NULL,
                               email TEXT NOT NULL UNIQUE,
                               created_utc TIMESTAMPTZ NOT NULL
                           );
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
