using Npgsql;
using Wonga.Services.UserProfile.Application;
using Wonga.Services.UserProfile.Domain;

namespace Wonga.Services.UserProfile.Infrastructure;

public sealed class UserProfileRepository(NpgsqlDataSource dataSource) : IUserProfileRepository
{
    public async Task UpsertAsync(UserProfileRecord profile, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO profile.user_profiles (user_id, first_name, last_name, email, created_utc)
                           VALUES (@user_id, @first_name, @last_name, @email, @created_utc)
                           ON CONFLICT (user_id)
                           DO UPDATE SET
                               first_name = EXCLUDED.first_name,
                               last_name = EXCLUDED.last_name,
                               email = EXCLUDED.email;
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("user_id", profile.UserId);
        command.Parameters.AddWithValue("first_name", profile.FirstName);
        command.Parameters.AddWithValue("last_name", profile.LastName);
        command.Parameters.AddWithValue("email", profile.Email);
        command.Parameters.AddWithValue("created_utc", profile.CreatedUtc.UtcDateTime);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<UserProfileRecord?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT user_id, first_name, last_name, email, created_utc
                           FROM profile.user_profiles
                           WHERE user_id = @user_id;
                           """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("user_id", userId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new UserProfileRecord
        {
            UserId = reader.GetGuid(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            Email = reader.GetString(3),
            CreatedUtc = DateTime.SpecifyKind(reader.GetDateTime(4), DateTimeKind.Utc)
        };
    }
}
