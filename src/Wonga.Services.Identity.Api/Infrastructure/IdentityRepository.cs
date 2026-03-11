using Npgsql;
using Wonga.Services.Identity.Application;
using Wonga.Services.Identity.Domain;

namespace Wonga.Services.Identity.Infrastructure;

public sealed class IdentityRepository(NpgsqlDataSource dataSource) : IIdentityRepository
{
    public async Task<IdentityUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT id, email, password_hash, password_salt, created_utc
                           FROM auth.users
                           WHERE email = @email;
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapUser(reader) : null;
    }

    public async Task CreateUserAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO auth.users (id, email, password_hash, password_salt, created_utc)
                           VALUES (@id, @email, @password_hash, @password_salt, @created_utc);
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", user.Id);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("password_salt", user.PasswordSalt);
        command.Parameters.AddWithValue("created_utc", user.CreatedUtc.UtcDateTime);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM auth.users WHERE id = @id;";

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", userId);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static IdentityUser MapUser(NpgsqlDataReader reader)
    {
        return new IdentityUser
        {
            Id = reader.GetGuid(0),
            Email = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            PasswordSalt = reader.GetString(3),
            CreatedUtc = DateTime.SpecifyKind(reader.GetDateTime(4), DateTimeKind.Utc)
        };
    }
}
