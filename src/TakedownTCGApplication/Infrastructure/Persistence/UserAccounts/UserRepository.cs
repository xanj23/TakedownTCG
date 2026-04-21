using Npgsql;
using TakedownTCGApplication.Abstractions;
using TakedownTCGApplication.Models.UserAccounts;

namespace TakedownTCGApplication.Infrastructure.Persistence.UserAccounts;

public sealed class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
        PostgreSqlDatabaseInitializer.EnsureSchema(_connectionString);
    }

    public async Task<bool> InsertUserAsync(User user)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            INSERT INTO users (user_name, user_email, password_hash, user_notifications)
            VALUES (@UserName, @UserEmail, @PasswordHash, @UserNotifications);
            """,
            connection);

        command.Parameters.AddWithValue("@UserName", user.UserName);
        command.Parameters.AddWithValue("@UserEmail", user.UserEmail);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@UserNotifications", user.UserNotifications);

        try
        {
            return await command.ExecuteNonQueryAsync() > 0;
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return false;
        }
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            SELECT user_name, user_email, password_hash, user_notifications
            FROM users
            WHERE LOWER(user_name) = LOWER(@UserName)
            LIMIT 1;
            """,
            connection);

        command.Parameters.AddWithValue("@UserName", userName);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadUser(reader) : null;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            SELECT user_name, user_email, password_hash, user_notifications
            FROM users
            WHERE LOWER(user_email) = LOWER(@UserEmail)
            LIMIT 1;
            """,
            connection);

        command.Parameters.AddWithValue("@UserEmail", email);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadUser(reader) : null;
    }

    public async Task<User?> GetByUserNameOrEmailAsync(string input)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            SELECT user_name, user_email, password_hash, user_notifications
            FROM users
            WHERE LOWER(user_name) = LOWER(@Input)
               OR LOWER(user_email) = LOWER(@Input)
            LIMIT 1;
            """,
            connection);

        command.Parameters.AddWithValue("@Input", input);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadUser(reader) : null;
    }

    public async Task<bool> UpdateUserNameAsync(string currentUserName, string newUserName)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            UPDATE users
            SET user_name = @NewUserName
            WHERE LOWER(user_name) = LOWER(@CurrentUserName);
            """,
            connection);

        command.Parameters.AddWithValue("@NewUserName", newUserName);
        command.Parameters.AddWithValue("@CurrentUserName", currentUserName);

        try
        {
            return await command.ExecuteNonQueryAsync() > 0;
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return false;
        }
    }

    public async Task<bool> UpdateEmailAsync(string userName, string newEmail)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            UPDATE users
            SET user_email = @NewEmail
            WHERE LOWER(user_name) = LOWER(@UserName);
            """,
            connection);

        command.Parameters.AddWithValue("@NewEmail", newEmail);
        command.Parameters.AddWithValue("@UserName", userName);

        try
        {
            return await command.ExecuteNonQueryAsync() > 0;
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return false;
        }
    }

    public async Task<bool> UpdatePasswordHashAsync(string userName, string newPasswordHash)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            UPDATE users
            SET password_hash = @PasswordHash
            WHERE LOWER(user_name) = LOWER(@UserName);
            """,
            connection);

        command.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
        command.Parameters.AddWithValue("@UserName", userName);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateNotificationsAsync(string userName, bool enabled)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            UPDATE users
            SET user_notifications = @Enabled
            WHERE LOWER(user_name) = LOWER(@UserName);
            """,
            connection);

        command.Parameters.AddWithValue("@Enabled", enabled);
        command.Parameters.AddWithValue("@UserName", userName);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteUserAsync(string userName)
    {
        await using NpgsqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using NpgsqlCommand command = new(
            """
            DELETE FROM users
            WHERE LOWER(user_name) = LOWER(@UserName);
            """,
            connection);

        command.Parameters.AddWithValue("@UserName", userName);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    private static User ReadUser(NpgsqlDataReader reader)
    {
        return new User
        {
            UserName = reader.GetString(0),
            UserEmail = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            UserNotifications = reader.GetBoolean(3)
        };
    }
}
