using Homework.Contracts;
using Homework.Models;
using Npgsql;
using NpgsqlTypes;

namespace Homework.Data;

public sealed class UserRepository(NpgsqlDataSource database)
{
    public async Task InitializeAsync()
    {
        await using var command = database.CreateCommand("""
            CREATE TABLE IF NOT EXISTS users (
                id uuid PRIMARY KEY,
                login varchar(50) NOT NULL,
                name varchar(100) NOT NULL,
                password_hash text NOT NULL,
                created_date timestamptz NOT NULL,
                updated_date timestamptz NOT NULL
            );
            CREATE UNIQUE INDEX IF NOT EXISTS ux_users_login ON users (lower(login));
            """);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<User?> FindAsync(string login)
    {
        await using var command = database.CreateCommand("SELECT * FROM users WHERE lower(login) = lower(@login)");
        command.Parameters.AddWithValue("login", login);
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Read(reader) : null;
    }

    public async Task<List<User>> GetAllAsync(UserFilter filter)
    {
        await using var command = database.CreateCommand("""
            SELECT * FROM users
            WHERE (@cf IS NULL OR created_date >= @cf) AND (@ct IS NULL OR created_date <= @ct)
              AND (@uf IS NULL OR updated_date >= @uf) AND (@ut IS NULL OR updated_date <= @ut)
            ORDER BY created_date, id
            """);
        AddDate(command, "cf", filter.CreatedFrom);
        AddDate(command, "ct", filter.CreatedTo);
        AddDate(command, "uf", filter.UpdatedFrom);
        AddDate(command, "ut", filter.UpdatedTo);
        await using var reader = await command.ExecuteReaderAsync();
        var users = new List<User>();
        while (await reader.ReadAsync()) users.Add(Read(reader));
        return users;
    }

    public async Task InsertAsync(User user)
    {
        await using var command = database.CreateCommand("""
            INSERT INTO users (id, login, name, password_hash, created_date, updated_date)
            VALUES (@id, @login, @name, @hash, @created, @updated)
            """);
        AddUser(command, user);
        command.Parameters.AddWithValue("created", user.CreatedDate);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> UpdateAsync(User user)
    {
        await using var command = database.CreateCommand("""
            UPDATE users SET login=@login, name=@name, password_hash=@hash, updated_date=@updated WHERE id=@id
            """);
        AddUser(command, user);
        return await command.ExecuteNonQueryAsync() == 1;
    }

    public async Task<bool> DeleteAsync(string login)
    {
        await using var command = database.CreateCommand("DELETE FROM users WHERE lower(login)=lower(@login)");
        command.Parameters.AddWithValue("login", login);
        return await command.ExecuteNonQueryAsync() == 1;
    }

    private static void AddDate(NpgsqlCommand command, string name, DateTimeOffset? value) =>
        command.Parameters.AddWithValue(name, NpgsqlDbType.TimestampTz, (object?)value?.UtcDateTime ?? DBNull.Value);

    private static void AddUser(NpgsqlCommand command, User user)
    {
        command.Parameters.AddWithValue("id", user.Id);
        command.Parameters.AddWithValue("login", user.Login);
        command.Parameters.AddWithValue("name", user.Name);
        command.Parameters.AddWithValue("hash", user.PasswordHash);
        command.Parameters.AddWithValue("updated", user.UpdatedDate);
    }

    private static User Read(NpgsqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("id")),
        Login = reader.GetString(reader.GetOrdinal("login")),
        Name = reader.GetString(reader.GetOrdinal("name")),
        PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
        CreatedDate = reader.GetDateTime(reader.GetOrdinal("created_date")),
        UpdatedDate = reader.GetDateTime(reader.GetOrdinal("updated_date"))
    };
}
